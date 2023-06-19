using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsTroubleShooter.View;

namespace WindowsTroubleShooter
{
    class SearchBarIssue
    {
        public DetectingIssue detectingIssue { get; private set; }
        public SearchBarIssue(DetectingIssue detectingIssue) { this.detectingIssue = detectingIssue; }

       
        public async Task verifiyRegistry()
            {


                const string userRoot = "HKEY_CURRENT_USER";
                const string keyName = userRoot + "\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Search";

                //Open the search function in the registry 
                using (Registry.CurrentUser.OpenSubKey(keyName, true))
                {

                await Task.Delay(1300);
                detectingIssue.repairs.Content = "Verifying the registry value";
                
                //The value doesn't exists
                if (!valueNameExists())
                {
                    detectingIssue.repairs.Content = "Adding Search function in the registry..";
                    Registry.SetValue(keyName, "BingSearchEnabled", 0x0000000u, RegistryValueKind.DWord);
                    await Task.Delay(1000);
                    detectingIssue.repairs.Content = "Key value has been added";
                    await Task.Delay(500);
                }
                //If the value is already there, goodluck...
                else
                {
                    detectingIssue.repairs.Content = "No issues in the registry";
                    await Task.Delay(1000);
                }

                await Task.Delay(300);

            }
                static bool valueNameExists()
                {
                    RegistryKey winLogonKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", true);
                    return (winLogonKey.GetValueNames().Contains("BingSearchEnabled"));
                }
            }

        
    }
}
