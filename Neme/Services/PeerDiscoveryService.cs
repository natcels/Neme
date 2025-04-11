using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using Neme.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Neme.Utils;

namespace Neme.Services
{
    public class PeerDiscoveryService
    { 
        private List<Peer> _peers = new List<Peer>();
        public event Action<List<Peer>> PeersUpdated;

        public async Task DiscoverPeersAsync()
        {
            //string localIP = GetLocalIPAddress();
            //if (string.IsNullOrEmpty(localIP)) return;

            //string baseIP = localIP.Substring(0, localIP.LastIndexOf('.') + 1);
            //List<Task> pingTasks = new List<Task>();

            //for (int i = 1; i < 255; i++)
            //{
            //    string ip = baseIP + i;
            //    pingTasks.Add(Task.Run(async () => await PingDevice(ip)));
            //}

            //await Task.WhenAll(pingTasks);

            //PeersUpdated?.Invoke(_peers);
            LoggerUtility.LogInfo("🔍 Starting peer discovery...");

            List<Peer> discoveredPeers = await ScanNetworkForPeers();


            LoggerUtility.LogInfo($"✅ Found {discoveredPeers.Count} peers.");

            if (discoveredPeers.Count > 0)
            {
                PeersUpdated?.Invoke(discoveredPeers);
            }
        }

        public List<Peer> GetConnectedDevices()
        {
            List<Peer> discoveredPeers = new List<Peer>();

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "arp",
                    Arguments = "-a",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Extract IPs and MAC addresses from ARP output
            Regex regex = new Regex(@"(\d+\.\d+\.\d+\.\d+)\s+([\w-]+)", RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(output);

            foreach (Match match in matches)
            {
                string ipAddress = match.Groups[1].Value;
                if (!ipAddress.StartsWith("0.0.0.") && !ipAddress.EndsWith(".255"))
                {
                    discoveredPeers.Add(new Peer { Address = ipAddress, Name = "Unknown" });
                }
            }

            return discoveredPeers;
        }
        private bool pingSuccess = false;

        private async Task PingDevice(string ip)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(ip, 500);

                if (reply.Status == IPStatus.Success)
                {
                    string hostName = GetHostName(ip);
                    _peers.Add(new Peer { 
                        Address = ip, 
                        Name = hostName });
                    pingSuccess = true;
                }
            }
            catch { /* Ignore exceptions */ }
        }

        private string GetHostName(string ip)
        {
            try
            {
                return Dns.GetHostEntry(ip).HostName;
            }
            catch
            {
                return ip; // Return IP if hostname is unavailable
            }
        }

        private string GetLocalIPAddress()
        {
            string localIP = null;
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public  async Task<List<Peer>> ScanNetworkForPeers()
        {
            List<Peer> peers = new List<Peer>();

            string localIP = NetworkUtility.GetLocalIPAddress();
            if (string.IsNullOrEmpty(localIP))
            {
                LoggerUtility.LogError("❌ Could not get local IP address.");
                return peers;
            }

            string baseIP = localIP.Substring(0, localIP.LastIndexOf('.') + 1);

            LoggerUtility.LogInfo($"🌍 Scanning network range: {baseIP}xxx");

            List<Task> pingTasks = new List<Task>();

            for (int i = 1; i < 255; i++)
            {
                string ip = baseIP + i;

                pingTasks.Add(Task.Run(async () =>
                {
                     PingDevice(ip);
                    bool isAlive = pingSuccess;
                    if (isAlive)
                    {
                        LoggerUtility.LogInfo($"✅ Found peer: {ip}");
                        lock (peers)
                        {
                            peers.Add(new Peer { Address = ip, Name = GetHostName(ip) });
                        }
                    }
                }));
            }

            await Task.WhenAll(pingTasks);

            LoggerUtility.LogInfo($"🔎 Scan complete. Total peers found: {peers.Count}");

            return peers;
        }

    }
}




