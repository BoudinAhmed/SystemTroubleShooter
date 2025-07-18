﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SystemTroubleShooter.Model.Troubleshooter
{
    public class InternetTroubleshooter : BaseTroubleshooter
    {

        private const string _checkAdaptersScriptPath = @"Scripts\\Internet\\IsNetworkAdaptersAvailable.ps1"; 
        private const string _refreshAdaptersScriptPath = @"Scripts\\Internet\\RefreshNetworkAdapter.ps1";
        private const string _pingScriptPath = @"Scripts\\Internet\\PingTests.ps1";

        private readonly List<TroubleshootingStep> _troubleshootingSteps;

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
                new() {
                    Description = "Checking Network Adapters Availability",
                    ScriptPath = _checkAdaptersScriptPath,
                    IsCritical = true // If adapters aren't available, can't proceed
                },
                new() {
                    Description = "Refreshing Network Adapters",
                    ScriptPath = _refreshAdaptersScriptPath,
                    IsCritical = false
                },
                new() {
                    Description = "Verifying Connection",
                    ScriptPath = _pingScriptPath,
                    IsCritical = true // if can't ping, troubleshooting failed
                }
            };
        }

        public override async Task<string> RunDiagnosticsAsync()
        {

            foreach (var step in _troubleshootingSteps)
            {

                
                (IsFixed, ResolutionMessage) =  await ExecuteTroubleshootingStepAsync(step);

                
                if (!IsFixed && step.IsCritical)
                    break;


            }
            return "";
        }
    }
}