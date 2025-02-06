using System;
using System.Net;
using System.Net.Sockets;
using WebSocketSharp.Server;

namespace Neme.Utils
{
    internal class SignalingServer : WebSocketBehavior
    {
        private static bool serverStarted;
        static WebSocketServer server;

        public static void StartServer(int defaultPort = 55667)
        {
            try
            {
                int portToUse = FindAvailablePort(defaultPort);
                server = new WebSocketServer($"ws://0.0.0.0:{portToUse}");
                server.AddWebSocketService<SignalingServer>("/signaling");
                server.Start();

                LoggerUtility.LogInfo($"Signaling server running on ws://0.0.0.0:{portToUse}");
                serverStarted = true;
            }
            catch (Exception ex)
            {
                serverStarted = false;
                LoggerUtility.LogError( ex);
            }
        }

        private static int FindAvailablePort(int preferredPort)
        {
            if (IsPortAvailable(preferredPort))
            {
                LoggerUtility.LogInfo($"Default port {preferredPort} is available.");
                return preferredPort;
            }

            // Randomize a port in the ephemeral range
            Random rand = new Random();
            int randomPort;

            do
            {
                randomPort = rand.Next(49152, 65536);
            } while (!IsPortAvailable(randomPort));

            LoggerUtility.LogInfo($"Default port {preferredPort} was unavailable. Using randomized port {randomPort}.");
            return randomPort;
        }

        private static bool IsPortAvailable(int port)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public static void StopServer()
        {
            if (serverStarted)
            {
                server.Stop();
                LoggerUtility.LogInfo("Signaling server stopped.");
                serverStarted = false;
            }
        }

        public static bool IsServerRunning()
        {
            return serverStarted && server.IsListening;
        }
    }
}
