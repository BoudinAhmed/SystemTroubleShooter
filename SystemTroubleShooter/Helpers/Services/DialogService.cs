using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SystemTroubleShooter.Interfaces;
using SystemTroubleShooter.View;
using SystemTroubleShooter.ViewModel;

namespace SystemTroubleShooter.Helpers.Services
{
    public class DialogService : IDialogService
    {
        void IDialogService.ShowCompletionMessageAsync(string message)
        {
            var completionMessageView = new CompletionMessageView()
            {
                DataContext = new CompletionMessageViewModel(message)
            };

            completionMessageView.Show();
        }

        
    }
}
