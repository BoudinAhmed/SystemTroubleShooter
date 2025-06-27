using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemTroubleShooter.Model.Troubleshooter
{
    public class BluethoothTroubleshooter : BaseTroubleshooter
    {
        private const string _restartBluetoothServicesScriptPath = @"Scripts\\Bluetooth\\RestartBluetoothServices.ps1";

        private readonly List<TroubleshootingStep> _troubleshootingSteps;

        public BluethoothTroubleshooter()
        {
            _troubleshootingSteps = new List<TroubleshootingStep>
            {
                new() {
                    Description = "Restarting Bluetooth services",
                    ScriptPath = _restartBluetoothServicesScriptPath,
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
