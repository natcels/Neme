using Neme.Models;
using Neme.Services;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Runtime.Intrinsics.Arm;
using System.Threading;
using System.Windows.Navigation;

namespace Neme.Utils
{
    internal class PeerManager
    {
        private readonly HeartbeatSender heartbeatSender;
        private readonly HeartbeatListener heartbeatListener;
        private readonly Dictionary<string, DateTime> peerLastSeen;
        private readonly int checkInterval = 10000;  // Check interval in milliseconds (10 seconds)
        private readonly int timeoutThreshold = 15000;  // Timeout threshold in milliseconds (15 seconds)
        private ChatMessage cm;
        private PeerDiscovery pd;
        private List<Peer> DiscoveredPeers;

        public PeerManager(string broadcastAddress, int port, string systemName)
        {
            heartbeatSender = new HeartbeatSender(broadcastAddress, port, systemName);
            heartbeatListener = new HeartbeatListener(port);
            peerLastSeen = new Dictionary<string, DateTime>();
            cm = new ChatMessage();
            pd = new PeerDiscovery(broadcastAddress, port, systemName);
            pd.StartListening(port);
            var dp = pd.GetDiscoveredPeers();
            foreach(var pr in dp)
            {
                DiscoveredPeers.Add(pr);
            }
            
        }

        public void Start()
        {
            // Start sending heartbeats
            heartbeatSender.StartSendingHeartbeats();
            ChatMessageService cms = new ChatMessageService();
                foreach   (var peer in DiscoveredPeers) {
                        cms.SendMessage(cm, peer);
                    }
            
            LoggerUtility.LogInfo("Heartbeats started");
            // Start listening for heartbeats
            heartbeatListener.StartListening();

            // Start monitoring peers
            Timer timer = new Timer(CheckPeerStatus, null, 0, checkInterval);
        }

        public void Stop()
        {
            
        }

        private void CheckPeerStatus(object state)
        {
            DateTime now = DateTime.Now;

            foreach (var peer in new List<string>(peerLastSeen.Keys))
            {
                if ((now - peerLastSeen[peer]).TotalMilliseconds > timeoutThreshold)
                {
                    LoggerUtility.LogWarning($"Peer {peer} is unresponsive.");
                    heartbeatListener.MarkPeerUnresponsive(peer);
                }
            }
        }

        public void UpdatePeerLastSeen(string systemName)
        {
            if (!peerLastSeen.ContainsKey(systemName))
            {
                peerLastSeen.Add(systemName, DateTime.Now);
            }
            else
            {
                peerLastSeen[systemName] = DateTime.Now;
            }
            LoggerUtility.LogInfo($"{systemName} last-seen updated");
        }
       
    }
}
