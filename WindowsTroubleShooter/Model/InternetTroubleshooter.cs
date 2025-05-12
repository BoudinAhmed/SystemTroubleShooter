using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading; // Add this using directive

namespace WindowsTroubleShooter.Model
{
    public class InternetTroubleshooter : BaseTroubleshooter
    {

        private const string CheckAdaptersScriptPath = @"Scripts\\Internet\\IsNetworkAdaptersAvailable.ps1"; 
        private const string RefreshAdaptersScriptPath = @"Scripts\\Internet\\RefreshNetworkAdapter.ps1";

        private string _networkadapter;
        private List<TroubleshootingStep> _troubleshootingSteps;

        public InternetTroubleshooter()
        {
            IssueType = "Internet Connection";
            Detail = "Fix problems with connecting to the internet";
            TimeStamp = DateTime.Now;
            TaskList = new List<string>
            {
                "Checking Internet Connection",
                "Refreshing Network Adapter",
                "Network Reset"
            };

            _troubleshootingSteps = new List<TroubleshootingStep>
            {
                new TroubleshootingStep {
                    Description = "Checking Network Adapters Availability",
                    ScriptPath = CheckAdaptersScriptPath,
                    IsCritical = true // If adapters aren't available, can't proceed
                },
                new TroubleshootingStep {
                    Description = "Refreshing Network Adapters",
                    ScriptPath = RefreshAdaptersScriptPath,
                    IsCritical = false
                } 
            };
        }

        public override async Task<string> RunDiagnosticsAsync()
        {

            foreach (var step in _troubleshootingSteps)
            {

                
                (IsFixed, ResolutionMessage) =  await ExecuteTroubleshootingStepAsync(step);

                
                if (step.IsCritical && !IsFixed)
                    break;


            }
            return ResolutionMessage;
        }
    }
}