using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SystemTroubleShooter.Model.Troubleshooter;

namespace SystemTroubleShooter.Test.Model
{
    public class InternetTroubleshooterTests
    {
        private List<BaseTroubleshooter.TroubleshootingStep> GetTroubleshootingSteps(InternetTroubleshooter troubleshooter)
        {
            var field = typeof(InternetTroubleshooter).GetField("_troubleshootingSteps",
                BindingFlags.NonPublic | BindingFlags.Instance);
            return field?.GetValue(troubleshooter) as List<BaseTroubleshooter.TroubleshootingStep> 
                ?? new List<BaseTroubleshooter.TroubleshootingStep>();
        }

        [Fact]
        public void Constructor_InitializesPropertiesAndStepsCorrectly()
        {
            // Arrange & Act
            var troubleshooter = new InternetTroubleshooter();
            var steps = GetTroubleshootingSteps(troubleshooter);

            // Assert
            Assert.Equal("Internet Connection", troubleshooter.IssueType);
            Assert.Equal("Fix problems with connecting to the internet", troubleshooter.Detail);
            Assert.Equal(3, troubleshooter.TaskList.Count); // "Checking...", "Refreshing...", "Network Reset"

            Assert.NotNull(steps);
            Assert.Equal(3, steps.Count);

            // Verify the specific steps defined in InternetTroubleshooter's constructor
            Assert.Equal("Checking Network Adapters Availability", steps[0].Description);
            Assert.True(steps[0].IsCritical);
            Assert.Contains("IsNetworkAdaptersAvailable.ps1", steps[0].ScriptPath);

            Assert.Equal("Refreshing Network Adapters", steps[1].Description);
            Assert.False(steps[1].IsCritical);
            Assert.Contains("RefreshNetworkAdapter.ps1", steps[1].ScriptPath);

            Assert.Equal("Verifying Connection", steps[2].Description);
            Assert.True(steps[2].IsCritical);
            Assert.Contains("PingTests.ps1", steps[2].ScriptPath);
        }

        [Fact]
        public async Task RunDiagnosticsAsync_AllStepsSucceed_ReturnsSuccessFromLastStep()
        {
            // Arrange
            // Mock InternetTroubleshooter to control ExecuteTroubleshootingStepAsync
            var mockTroubleshooter = new Mock<InternetTroubleshooter>() { CallBase = true }; // CallBase = true to run original constructor and other non-mocked methods

            // Setup the mock to return success for any step
            mockTroubleshooter.Protected() // Moq.Protected for protected virtual members
                .Setup<Task<(bool IsSuccess, string Message)>>(
                    "ExecuteTroubleshootingStepAsync", // Name of the protected method
                    ItExpr.IsAny<BaseTroubleshooter.TroubleshootingStep>() // Match any step
                )
                .ReturnsAsync((BaseTroubleshooter.TroubleshootingStep step) => (true, $"'{step.Description}' Mocked Success."));

            var troubleshooter = mockTroubleshooter.Object; // Get the instance

            // Act
            string result = await troubleshooter.RunDiagnosticsAsync();

            // Assert
            Assert.True(troubleshooter.IsFixed);
            Assert.Contains("'Verifying Connection' Mocked Success.", result); // Message from the last step defined in InternetTroubleshooter

            // Verify that ExecuteTroubleshootingStepAsync was called for all 3 steps
            var definedSteps = GetTroubleshootingSteps(troubleshooter); // Get steps from the actual instance
            foreach (var definedStep in definedSteps)
            {
                mockTroubleshooter.Protected().Verify(
                    "ExecuteTroubleshootingStepAsync",
                    Times.Once(), // Ensure it was called exactly once for each step
                    ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedStep.Description)
                );
            }
        }
        [Fact]
        public async Task RunDiagnosticsAsync_CriticalStepFails_StopsAndReturnsFailureMessage()
        {
            // Arrange
            var mockTroubleshooter = new Mock<InternetTroubleshooter>() { CallBase = true };
            var definedSteps = GetTroubleshootingSteps(mockTroubleshooter.Object); // Get steps to know their order and criticality

            // First step (Critical): "Checking Network Adapters Availability" -> Fails
            mockTroubleshooter.Protected()
                .Setup<Task<(bool IsSuccess, string Message)>>(
                    "ExecuteTroubleshootingStepAsync",
                    ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedSteps[0].Description)
                )
                .ReturnsAsync((false, $"'{definedSteps[0].Description}' Mocked Critical Fail."));

            // Subsequent steps should not be called, but setup won't hurt (or make it throw if called)
            mockTroubleshooter.Protected()
                .Setup<Task<(bool IsSuccess, string Message)>>(
                    "ExecuteTroubleshootingStepAsync",
                    ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedSteps[1].Description)
                )
                .ReturnsAsync((true, $"'{definedSteps[1].Description}' Mocked Success but should not be called."));


            var troubleshooter = mockTroubleshooter.Object;

            // Act
            string result = await troubleshooter.RunDiagnosticsAsync();

            // Assert
            Assert.False(troubleshooter.IsFixed);
            Assert.Contains($"'{definedSteps[0].Description}' Mocked Critical Fail.", result);

            // Verify the first (failing) step was called
            mockTroubleshooter.Protected().Verify(
                "ExecuteTroubleshootingStepAsync",
                Times.Once(),
                ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedSteps[0].Description)
            );

            // Verify the second step (and third) were NOT called because the first critical one failed
            mockTroubleshooter.Protected().Verify(
                "ExecuteTroubleshootingStepAsync",
                Times.Never(),
                ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedSteps[1].Description)
            );
            mockTroubleshooter.Protected().Verify(
                "ExecuteTroubleshootingStepAsync",
                Times.Never(),
                ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedSteps[2].Description)
            );
        }

        [Fact]
        public async Task RunDiagnosticsAsync_NonCriticalStepFails_ContinuesAndReturnsSuccessFromLastStep()
        {
            // Arrange
            var mockTroubleshooter = new Mock<InternetTroubleshooter>() { CallBase = true };
            var definedSteps = GetTroubleshootingSteps(mockTroubleshooter.Object);

            // Step 1 (Critical): "Checking Network Adapters Availability" -> Succeeds
            mockTroubleshooter.Protected()
                .Setup<Task<(bool IsSuccess, string Message)>>(
                    "ExecuteTroubleshootingStepAsync",
                    ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedSteps[0].Description)
                )
                .ReturnsAsync((true, $"'{definedSteps[0].Description}' Mocked Success."));

            // Step 2 (Non-Critical): "Refreshing Network Adapters" -> Fails
            mockTroubleshooter.Protected()
                .Setup<Task<(bool IsSuccess, string Message)>>(
                    "ExecuteTroubleshootingStepAsync",
                    ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedSteps[1].Description)
                )
                .ReturnsAsync((false, $"'{definedSteps[1].Description}' Mocked Non-Critical Fail."));


            // Step 3 (Critical): "Verifying Connection" -> Succeeds
            mockTroubleshooter.Protected()
                .Setup<Task<(bool IsSuccess, string Message)>>(
                    "ExecuteTroubleshootingStepAsync",
                    ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedSteps[2].Description)
                )
                .ReturnsAsync((true, $"'{definedSteps[2].Description}' Mocked Success."));

            var troubleshooter = mockTroubleshooter.Object;

            // Act
            string result = await troubleshooter.RunDiagnosticsAsync();

            // Assert
            Assert.True(troubleshooter.IsFixed); // IsFixed is true because the last step encountered succeeded
            Assert.Contains($"'{definedSteps[2].Description}' Mocked Success.", result); // Message from the last step

            // Verify all steps were called
            foreach (var definedStep in definedSteps)
            {
                mockTroubleshooter.Protected().Verify(
                    "ExecuteTroubleshootingStepAsync",
                    Times.Once(),
                    ItExpr.Is<BaseTroubleshooter.TroubleshootingStep>(s => s.Description == definedStep.Description)
                );
            }
        }
        }
}
