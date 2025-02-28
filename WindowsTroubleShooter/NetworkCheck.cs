using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTroubleShooter
{
    internal class NetworkCheck
    {
        // Method to check if the internet is available
        internal static bool IsInternetAvailable()
        {
            try
            {
                // Create a new Ping instance
                using (Ping ping = new Ping())
                {
                    // Send a ping to www.google.com with a timeout of 2000 ms
                    PingReply reply = ping.Send("www.google.com", 2000); // 3000 ms timeout
                    // Return true if the ping was successful
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                // Return false if an exception occurs (e.g., no internet connection)
                return false;
            }
        }
    }
}
