using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Neme.Models;

namespace Neme.Utils
{
    internal class PeerDiscovery
    {
        private UdpClient udpClient;
        private IPEndPoint endPoint;
        //  private readonly HashSet<string> discoveredPeers; // Store unique peer names
        private List<Peer> discoveredPeers;

        public PeerDiscovery(string broadcastAddress, int port, string key)
        {
            udpClient = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Parse(broadcastAddress), port);
        }

        // Encrypts and sends the system name as a discovery message
        public void StartBroadcasting()
        {
            string systemName = Environment.MachineName;
            string encryptedMessage = AesEncryption.Encrypt(systemName);
            byte[] data = Encoding.ASCII.GetBytes(encryptedMessage);
            udpClient.Send(data, data.Length, endPoint);
        //    LoggerUtility.LogInfo("Discovery message broadcasted.");
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
            string decryptedMessage = AesEncryption.Decrypt(encryptedMessage);

            LoggerUtility.LogInfo("Received decrypted message: " + decryptedMessage);
            ChatMessage M = JsonSerializer.Deserialize<ChatMessage>(decryptedMessage);
            Peer p = new();
            
            // Add peer to the list if not already present
            if (!discoveredPeers.Contains(p))
            {
                               
                discoveredPeers.Add(p);
                LoggerUtility.LogInfo($"Peer added: {decryptedMessage}");
            }

            // Continue listening for more peers
            udpClient.BeginReceive(ReceiveCallback, null);
        }

        // Returns a list of discovered peers
        public List<Peer> GetDiscoveredPeers()
        {
            return discoveredPeers;
        }
    }
}
