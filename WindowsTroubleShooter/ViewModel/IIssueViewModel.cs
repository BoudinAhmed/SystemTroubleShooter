using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTroubleShooter.ViewModel
{
    public interface IIssueViewModel
    {
        public string StatusMessage { get; }
        Task RunDiagnosticsAsync();
    }
}
