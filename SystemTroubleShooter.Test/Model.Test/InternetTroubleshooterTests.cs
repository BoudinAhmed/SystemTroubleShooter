using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit; // Import Xunit framework for testing

namespace SystemTroubleShooter.Model.Troubleshooter.Tests
{
    // A testable version of InternetTroubleshooter that allows mocking of ExecuteTroubleshootingStepAsync
    public class TestableInternetTroubleshooter : InternetTroubleshooter
    {
        // A queue to store predefined results for each call to ExecuteTroubleshootingStepAsync
        private readonly Queue<(bool IsSuccess, string Message)> _mockResults = new();

        /// <summary>
        /// Enqueues a mock result that will be returned by the next call to ExecuteTroubleshootingStepAsync.
        /// </summary>
        /// <param name="isSuccess">The success status to return.</param>
        /// <param name="message">The message to return.</param>
        public void EnqueueMockResult(bool isSuccess, string message)
        {
            _mockResults.Enqueue((isSuccess, message));
        }

        /// <summary>
        /// Overrides the base method to return a predefined mock result from the queue.
        /// This bypasses actual PowerShell execution and UI dispatcher calls for unit testing.
        /// </summary>
        protected override Task<(bool IsSuccess, string Message)> ExecuteTroubleshootingStepAsync(TroubleshootingStep step)
        {
            if (_mockResults.Count == 0)
            {
                // If no mock results are enqueued, it means the test setup is incomplete.
                throw new InvalidOperationException($"No mock result enqueued for step: {step.Description}. Ensure all expected steps have a corresponding EnqueueMockResult call in the test.");
            }
            // Dequeue and return the next mock result
            return Task.FromResult(_mockResults.Dequeue());
        }
    }

    /// <summary>
    /// Contains XUnit tests for the InternetTroubleshooter class.
    /// </summary>
    public class InternetTroubleshooterTests
    {
        /// <summary>
        /// Tests that the constructor correctly initializes properties.
        /// </summary>
        [Fact]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            // Arrange & Act
            var troubleshooter = new InternetTroubleshooter();

            // Assert
            Assert.Equal("Internet Connection", troubleshooter.IssueType);
            Assert.Equal("Fix problems with connecting to the internet", troubleshooter.Detail);
            Assert.True(troubleshooter.TimeStamp <= DateTime.Now); // Check if timestamp is set close to now
            Assert.NotNull(troubleshooter.TaskList);
            Assert.Contains("Checking Internet Connection", troubleshooter.TaskList);
            Assert.Contains("Refreshing Network Adapter", troubleshooter.TaskList);
            Assert.Contains("Network Reset", troubleshooter.TaskList);
        }

        /// <summary>
        /// Tests the scenario where all troubleshooting steps succeed.
        /// </summary>
        [Fact]
        public async Task RunDiagnosticsAsync_AllStepsSucceed_ReturnsIsFixedTrue()
        {
            // Arrange
            var troubleshooter = new TestableInternetTroubleshooter();

            // Enqueue mock success results for each step in order
            troubleshooter.EnqueueMockResult(true, "Network adapters are available."); // IsNetworkAdaptersAvailable.ps1
            troubleshooter.EnqueueMockResult(true, "Network adapter refreshed successfully."); // RefreshNetworkAdapter.ps1
            troubleshooter.EnqueueMockResult(true, "All essential network connectivity tests passed successfully."); // PingTests.ps1

            // Act
            await troubleshooter.RunDiagnosticsAsync();

            // Assert
            Assert.True(troubleshooter.IsFixed); // Expect the issue to be fixed
            Assert.Equal("All essential network connectivity tests passed successfully.", troubleshooter.ResolutionMessage); // Expect message from the last successful step
        }

        /// <summary>
        /// Tests the scenario where a critical troubleshooting step fails.
        /// The process should stop immediately after the critical failure.
        /// </summary>
        [Fact]
        public async Task RunDiagnosticsAsync_CriticalStepFails_StopsAndReturnsIsFixedFalse()
        {
            // Arrange
            var troubleshooter = new TestableInternetTroubleshooter();

            // Enqueue a failure for the first critical step (Checking Network Adapters)
            troubleshooter.EnqueueMockResult(false, "No active network adapters found."); // IsNetworkAdaptersAvailable.ps1 (Critical, Fails)
            // Even if we enqueue more, they shouldn't be executed due to critical failure
            troubleshooter.EnqueueMockResult(true, "Should not be reached."); // RefreshNetworkAdapter.ps1
            troubleshooter.EnqueueMockResult(true, "Should not be reached either."); // PingTests.ps1


            // Act
            await troubleshooter.RunDiagnosticsAsync();

            // Assert
            Assert.False(troubleshooter.IsFixed); // Expect the issue to NOT be fixed
            Assert.Equal("No active network adapters found.", troubleshooter.ResolutionMessage); // Expect message from the failed critical step
        }

        /// <summary>
        /// Tests the scenario where a non-critical step fails, but a subsequent critical step succeeds.
        /// The process should continue, and the final status should reflect the last critical step.
        /// </summary>
        [Fact]
        public async Task RunDiagnosticsAsync_NonCriticalFailsThenCriticalSucceeds_ReturnsIsFixedTrue()
        {
            // Arrange
            var troubleshooter = new TestableInternetTroubleshooter();

            // 1. Critical step succeeds
            troubleshooter.EnqueueMockResult(true, "Network adapters are available."); // IsNetworkAdaptersAvailable.ps1

            // 2. Non-critical step fails
            troubleshooter.EnqueueMockResult(false, "Failed to refresh adapter (non-critical)."); // RefreshNetworkAdapter.ps1

            // 3. Critical step succeeds (this will set the final IsFixed status)
            troubleshooter.EnqueueMockResult(true, "All essential network connectivity tests passed successfully."); // PingTests.ps1

            // Act
            await troubleshooter.RunDiagnosticsAsync();

            // Assert
            Assert.True(troubleshooter.IsFixed); // Expect the issue to be fixed due to the final critical step succeeding
            Assert.Equal("All essential network connectivity tests passed successfully.", troubleshooter.ResolutionMessage); // Expect message from the last successful step
        }

        /// <summary>
        /// Tests the scenario where the last critical step fails.
        /// </summary>
        [Fact]
        public async Task RunDiagnosticsAsync_LastCriticalStepFails_ReturnsIsFixedFalse()
        {
            // Arrange
            var troubleshooter = new TestableInternetTroubleshooter();

            // 1. Critical step succeeds
            troubleshooter.EnqueueMockResult(true, "Network adapters are available."); // IsNetworkAdaptersAvailable.ps1

            // 2. Non-critical step succeeds
            troubleshooter.EnqueueMockResult(true, "Network adapter refreshed."); // RefreshNetworkAdapter.ps1

            // 3. Critical step fails (Verifying Connection)
            troubleshooter.EnqueueMockResult(false, "Network connectivity issues detected. Failed to ping 8.8.8.8."); // PingTests.ps1 (Critical, Fails)

            // Act
            await troubleshooter.RunDiagnosticsAsync();

            // Assert
            Assert.False(troubleshooter.IsFixed); // Expect the issue to NOT be fixed
            Assert.Equal("Network connectivity issues detected. Failed to ping 8.8.8.8.", troubleshooter.ResolutionMessage); // Expect message from the failed critical step
        }
    }
}