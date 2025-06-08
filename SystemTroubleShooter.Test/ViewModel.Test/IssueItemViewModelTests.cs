using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemTroubleShooter.Model.Troubleshooter;
using SystemTroubleShooter.Test.Mocks;
using SystemTroubleShooter.ViewModel;

namespace SystemTroubleShooter.Test.ViewModel.Test
{
    public class IssueItemViewModelTests
    {
        private readonly MockAudioDeviceDisplayService _mockAudioService;
        private readonly MockDialogService _mockDialogService;
        private readonly IssueItemViewModel _viewModel;

        public IssueItemViewModelTests()
        {
            _mockAudioService = new MockAudioDeviceDisplayService();
            _mockDialogService = new MockDialogService();
            _viewModel = new IssueItemViewModel(_mockAudioService, _mockDialogService);
        }

        [Fact]
        public void InitialState_IsCorrect()
        {
            Assert.False(_viewModel.AreButtonsVisible);
            Assert.True(_viewModel.IsTextVisible);
            Assert.False(_viewModel.IsTroubleshooting);
            Assert.Null(_viewModel.TroubleshootingStatus);
            Assert.False(_viewModel.IsItemSelected);
        }

        [Fact]
        public void ItemClickedCommand_WhenTroubleshooting_DoesNothing()
        {
            _viewModel.IsTroubleshooting = true;
            var initialIsItemSelected = _viewModel.IsItemSelected;

            _viewModel.ItemClickedCommand.Execute(null);

            Assert.Equal(initialIsItemSelected, _viewModel.IsItemSelected);
        }

        [Fact]
        public void ItemCancelClickedCommand_ResetsState()
        {
            

            _viewModel.ItemCancelClickedCommand.Execute(null);

            Assert.False(_viewModel.IsItemSelected);
            Assert.True(_viewModel.IsTextVisible);
            Assert.False(_viewModel.AreButtonsVisible);
            Assert.False(_viewModel.IsTroubleshooting);
        }

        [Fact]
        public void OnItemTroubleshootClicked_WhenIssueTypeIsNull_ShowsComingSoon()
        {
            _viewModel.IssueType = null;

            _viewModel.ItemTroubleshootClickedCommand.Execute(null);

            Assert.True(_viewModel.IsTroubleshooting);
            Assert.Equal("Coming soon - Under Development", _viewModel.TroubleshootingStatus);
        }


        [Fact]
        public void IssueStatusChanged_UpdatesTroubleshootingStatus()
        {
            var mockTroubleshooter = new MockSoundTroubleshooter();
            _viewModel.IssueType = mockTroubleshooter;
            _viewModel.ItemTroubleshootClickedCommand.Execute(null);

            mockTroubleshooter.StatusMessage = "New status";
            mockTroubleshooter.OnPropertyChanged(nameof(BaseTroubleshooter.StatusMessage));

            Assert.Equal("New status", _viewModel.TroubleshootingStatus);
        }

        [Fact]
        public void ShowButtons_UpdatesVisibility()
        {
            _viewModel.ShowButtons();

            Assert.True(_viewModel.IsItemSelected);
            Assert.False(_viewModel.IsTextVisible);
            Assert.True(_viewModel.AreButtonsVisible);
        }

        [Fact]
        public void ResetIssueItem_ResetsState()
        {
            

            _viewModel.ResetIssueItem();

            Assert.False(_viewModel.IsItemSelected);
            Assert.True(_viewModel.IsTextVisible);
            Assert.False(_viewModel.AreButtonsVisible);
            Assert.False(_viewModel.IsTroubleshooting);


        }
    }
}