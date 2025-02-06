using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Neme.Utils
{
    internal class PeerDiscovery
    {
        private UdpClient udpClient;
        private IPEndPoint endPoint;
        private readonly AesEncryption aesEncryption;
        private readonly HashSet<string> discoveredPeers; // Store unique peer names

        public PeerDiscovery(string broadcastAddress, int port, string key)
        {
            udpClient = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Parse(broadcastAddress), port);
            aesEncryption = new AesEncryption(key);
            discoveredPeers = new HashSet<string>();  // Store discovered peers
        }

        // Encrypts and sends the system name as a discovery message
        public void StartBroadcasting()
        {
            string systemName = Environment.MachineName;
            string encryptedMessage = aesEncryption.Encrypt(systemName);
            byte[] data = Encoding.ASCII.GetBytes(encryptedMessage);
            udpClient.Send(data, data.Length, endPoint);
        //    Console.WriteLine("Discovery message broadcasted.");
        }

        // Start listening for discovery messages and decrypt them
        public void StartListening(int port)
        {
            udpClient = new UdpClient(port);
            udpClient.BeginReceive(ReceiveCallback, null);
        }

        // Callback method to handle incoming messages
        private void ReceiveCallback(IAsyncResult ar)
        {
            byte[] data = udpClient.EndReceive(ar, ref endPoint);
            string encryptedMessage = Encoding.ASCII.GetString(data);
            string decryptedMessage = aesEncryption.Decrypt(encryptedMessage);

            Console.WriteLine("Received decrypted message: " + decryptedMessage);

            // Add peer to the list if not already present
            if (!discoveredPeers.Contains(decryptedMessage))
            {
                discoveredPeers.Add(decryptedMessage);
                Console.WriteLine($"Peer added: {decryptedMessage}");
            }

            // Continue listening for more peers
            udpClient.BeginReceive(ReceiveCallback, null);
        }

        // Returns a list of discovered peers
        public List<string> GetDiscoveredPeers()
        {
            return new List<string>(discoveredPeers);
        }
    }
}
