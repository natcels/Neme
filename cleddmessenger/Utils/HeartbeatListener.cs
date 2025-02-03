using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace cleddmessenger.Utils
{
    internal class HeartbeatListener
    {
        private readonly UdpClient udpClient;
        private readonly int port;
        private readonly Dictionary<string, PeerStatus> peers;  // Dictionary to store peer status

        public HeartbeatListener(int port)
        {
            udpClient = new UdpClient(port);
            this.port = port;
            peers = new Dictionary<string, PeerStatus>();
        }

        public void StartListening()
        {
            udpClient.BeginReceive(ReceiveHeartbeatCallback, null);
        }

    

        private void UpdatePeerStatus(string systemName, PeerStatus status)
        {
            if (!peers.ContainsKey(systemName))
            {
                peers.Add(systemName, PeerStatus.Active);
            }
            else
            {
                peers[systemName] = status;
            }

            Console.WriteLine($"Peer {systemName} status: {status}");
        }

        public void MarkPeerUnresponsive(string systemName)
        {
            if (peers.ContainsKey(systemName))
            {
                peers[systemName] = PeerStatus.Unresponsive;
                Console.WriteLine($"Peer {systemName} is marked as unresponsive.");
            }
        }

        private readonly PeerManager peerManager;

        public HeartbeatListener(int port, PeerManager manager)
        {
            udpClient = new UdpClient(port);
            this.port = port;
            peers = new Dictionary<string, PeerStatus>();
            peerManager = manager;
        }

        private void ReceiveHeartbeatCallback(IAsyncResult ar)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            byte[] data = udpClient.EndReceive(ar, ref endPoint);
            string message = Encoding.ASCII.GetString(data);

            if (message.Contains(":heartbeat"))
            {
                string systemName = message.Split(':')[0];
                peerManager.UpdatePeerLastSeen(systemName);  // Notify PeerManager
                UpdatePeerStatus(systemName, PeerStatus.Active);
            }

            udpClient.BeginReceive(ReceiveHeartbeatCallback, null);
        }

    }

    public enum PeerStatus
    {
        Active,
        Unresponsive,
        Offline
    }
}
