using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Neme.Utils
{
    public static class NetworkUtility
    {
        /// <summary>
        /// Gets the local IP address of the current machine.
        /// </summary>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList
                       .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
                       .ToString() ?? "127.0.0.1"; // Default to localhost if no valid IP is found
        }
    }
}
