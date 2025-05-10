using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsTroubleShooter.View;
using WindowsTroubleShooter.ViewModel;

namespace WindowsTroubleShooter.Test
{


    public class StartViewModelTests
    {
        [Fact]
        public void SwitchToDashboardCommand_SetsCurrentUserControlToDashboardView()
        {
            // Arrange
            var viewModel = new StartViewModel();
            viewModel.SwitchToHomeCommand.Execute(null); // Navigate away

            // Act
            viewModel.SwitchToSystemOverviewCommand.Execute(null); // Switch back to Dashboard

            // Assert
            Assert.IsType<SystemOverviewViewModel>(viewModel.CurrentContentViewModel);
            Assert.Equal("Dashboard", viewModel.SelectedView);
        }

        [Fact]
        public void SwitchToProblemListCommand_SetsCurrentContentViewModelToProblemListViewModel()
        {
            // Arrange
            var viewModel = new StartViewModel();

            // Act
            viewModel.SwitchToHomeCommand.Execute(null); // Switch to ProblemList

            // Assert
            Assert.IsType<HomeViewModel>(viewModel.CurrentContentViewModel);
            Assert.Equal("ProblemList", viewModel.SelectedView);
        }

        [Fact]
        public void SwitchToSettingsCommand_SetsCurrentContentViewModelToSettingsViewModel()
        {
            // Arrange
            var viewModel = new StartViewModel();

            // Act
            viewModel.SwitchToSettingsCommand.Execute(null); // Switch to Settings

            // Assert
            Assert.IsType<SettingsViewModel>(viewModel.CurrentContentViewModel);
            Assert.Equal("Settings", viewModel.SelectedView);
        }

        [Fact]
        public void SwitchToAboutCommand_SetsCurrentContentViewModelToAboutViewModel()
        {
            // Arrange
            var viewModel = new StartViewModel();

            // Act
            viewModel.SwitchToAboutCommand.Execute(null); // Switch to About

            // Assert
            Assert.IsType<AboutViewModel>(viewModel.CurrentContentViewModel);
            Assert.Equal("About", viewModel.SelectedView);
        }

    }
}
