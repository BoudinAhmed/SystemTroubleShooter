using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTroubleShooter.Model.Troubleshooter
{
    public class WindowsUpdateTroubleshooter : BaseTroubleshooter
    {
        private const string _clearWindowsUpdateCacheScriptPath = @"Scripts\\WindowsUpdate\\Clear-WindowsUpdateCache.ps1";

        private readonly List<TroubleshootingStep> _troubleshootingSteps;

        public WindowsUpdateTroubleshooter()
        {
            _troubleshootingSteps = new List<TroubleshootingStep>
            {
                new() {
                    Description = "Clearing Windows Update cache",
                    ScriptPath = _clearWindowsUpdateCacheScriptPath,
                    IsCritical = false
                }
            };
        }

        public async override Task<string> RunDiagnosticsAsync()
        {
            foreach (var step in _troubleshootingSteps)
            {
               (IsFixed, ResolutionMessage) = await ExecuteTroubleshootingStepAsync(step);

                if (!IsFixed && step.IsCritical)
                    break;

            }
            
            return "";
        }
    }
}
