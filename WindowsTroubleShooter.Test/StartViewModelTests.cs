using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsTroubleShooter.View;
using WindowsTroubleShooter.ViewModel;

namespace WindowsTroubleShooter.Test
{
   

    public class StartViewModelTests
    {
        [WpfFact] // Runs the test in an STA thread automatically
        public void SwitchToProblemList_ShouldUpdateCurrentUserControl()
        {
            // Arrange
            var startViewModel = new StartViewModel();

            // Act
            startViewModel.SwitchToProblemListCommand.Execute(null);

            // Assert
            Assert.IsType<ProblemListView>(startViewModel.CurrentUserControl);
            Assert.Equal("ProblemList", startViewModel.SelectedView);
        }
    }
}
