using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace cleddmessenger.Utils
{
    internal class HeartbeatSender
    {
        private readonly UdpClient udpClient;
        private readonly IPEndPoint endPoint;
        private readonly string systemName;
        private readonly int interval = 5000;  // Heartbeat interval in milliseconds (5 seconds)

        public HeartbeatSender(string broadcastAddress, int port, string systemName)
        {
            udpClient = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Parse(broadcastAddress), port);
            this.systemName = systemName;
        }

        public void StartSendingHeartbeats()
        {
            Timer timer = new Timer(SendHeartbeat, null, 0, interval);
        }

        private void SendHeartbeat(object state)
        {
            string message = $"{systemName}:heartbeat";  // Example message
            byte[] data = Encoding.ASCII.GetBytes(message);
            udpClient.Send(data, data.Length, endPoint);
            Console.WriteLine($"Heartbeat sent from {systemName}");
        }
    }
}
