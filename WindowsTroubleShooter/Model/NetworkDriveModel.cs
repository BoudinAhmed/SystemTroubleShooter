using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTroubleShooter.Model
{
    public class NetworkDriveModel
    {
        public string MapNetworkDrive(char letter, string path)
        {
            string cmd = "net";
            string command = @$"use {letter}: {path}";
            
            try
            {
                //open cmd and map network drive
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = cmd;
                p.StartInfo.Arguments = $" {command}";
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();
                string output = p.StandardOutput.ReadToEnd();
                p.Dispose();

                return $"Netword drive {letter} mapped successfully";
            }

            catch 
            {
                return $"Failed to map {letter}. Please verify your internet connection and/or VPN if working from home";
            }
        }
    }
}
