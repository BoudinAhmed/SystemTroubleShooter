using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SystemTroubleShooter.Model.Troubleshooter;

namespace SystemTroubleShooter.Test.Mocks
{
    public class MockSoundTroubleshooter : SoundTroubleshooter
    {
        public bool RunDiagnosticsCalled { get; private set; }

        public override Task<string> RunDiagnosticsAsync()
        {
            RunDiagnosticsCalled = true;
            StatusMessage = "Diagnostics running";
            IsFixed = true;
            ResolutionMessage = "Sound issue fixed.";
            return Task.FromResult("Diagnostics Completed");
        }

        public override Task<(List<string> InputDevices, List<string> OutputDevices)> GetAllAudioDevicesAsync()
        {
            var input = new List<string> { "Input 1" };
            var output = new List<string> { "Output 1" };
            return Task.FromResult((input, output));
        }
    }
}
