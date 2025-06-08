using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemTroubleShooter.Interfaces;

namespace SystemTroubleShooter.Test.Mocks
{
    public class MockDialogService : IDialogService
    {
        public string? CompletionMessage { get; private set; }

        public void ShowCompletionMessageAsync(string message)
        {
            CompletionMessage = message;
        }
    }
}
