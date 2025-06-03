using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTroubleShooter.ViewModel
{
    public class CompletionMessageViewModel 
    {
        public string Message { get; set; } = "no message to display.";

        public CompletionMessageViewModel()
        {
        }
        public CompletionMessageViewModel(string message)
        {
            Message = message;
        }
    }
}
