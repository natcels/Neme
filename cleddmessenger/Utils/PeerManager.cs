using System;
using System.Collections.Generic;
using System.Threading;

namespace cleddmessenger.Utils
{
    internal class PeerManager
    {
        private readonly HeartbeatSender heartbeatSender;
        private readonly HeartbeatListener heartbeatListener;
        private readonly Dictionary<string, DateTime> peerLastSeen;
        private readonly int checkInterval = 10000;  // Check interval in milliseconds (10 seconds)
        private readonly int timeoutThreshold = 15000;  // Timeout threshold in milliseconds (15 seconds)

        public PeerManager(string broadcastAddress, int port, string systemName)
        {
            heartbeatSender = new HeartbeatSender(broadcastAddress, port, systemName);
            heartbeatListener = new HeartbeatListener(port);
            peerLastSeen = new Dictionary<string, DateTime>();
        }

        public void Start()
        {
            // Start sending heartbeats
            heartbeatSender.StartSendingHeartbeats();
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
