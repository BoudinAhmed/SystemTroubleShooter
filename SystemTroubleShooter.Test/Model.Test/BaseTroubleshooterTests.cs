using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemTroubleShooter.Model.Troubleshooter;

namespace SystemTroubleShooter.Test.Model.Test
{
    public class BaseTroubleshooterTests
    {
        
            private class TestTroubleshooter : BaseTroubleshooter
            {
                public override Task<string> RunDiagnosticsAsync()
                {
                    return Task.FromResult("Diagnostics Completed");
                }
            }

            [Fact]
            public void StatusMessage_Should_Update_Correctly()
            {
                var troubleshooter = new TestTroubleshooter();
                troubleshooter.StatusMessage = "Testing...";
                Assert.Equal("Testing...", troubleshooter.StatusMessage);
            }

            [Fact]
            public async Task RunDiagnosticsAsync_Should_Return_CorrectMessage()
            {
                var troubleshooter = new TestTroubleshooter();
                string result = await troubleshooter.RunDiagnosticsAsync();
                Assert.Equal("Diagnostics Completed", result);
            }

        }
    
}
