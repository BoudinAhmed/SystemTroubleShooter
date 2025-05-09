using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using WindowsTroubleShooter;
using WindowsTroubleShooter.ViewModels;


namespace WindowsTroubleShooter.Tests
{
    public class StartViewModelTests
    {
        [Fact]
        public void TestSwitchToDashboard()
        {
            // Arrange
            var viewModel = new StartViewModel();
            var expectedView = "DashboardView";
            // Act
            viewModel.SwitchToDashboardCommand.Execute(null);
            // Assert
            Assert.Equal(expectedView, viewModel.SelectedView);
        }
    }
}
