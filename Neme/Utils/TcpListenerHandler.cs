﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Neme.Utils
{
    internal class TcpListenerHandler
    {
        private TcpListener listener;

        public TcpListenerHandler(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void StartListening()
        {
            try
            {
                listener.Start();
                LoggerUtility.LogInfo("Listening for connections...");
                listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
            }
            catch (Exception ex)
            {
                LoggerUtility.LogInfo("Error starting listener: " + ex.Message);
            }
        }

        private void AcceptTcpClientCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient client = listener.EndAcceptTcpClient(ar);
                Task.Run(() => HandleClient(client));

                // Continue listening for other clients
                listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
            }
            catch (Exception ex)
            {
                LoggerUtility.LogInfo("Error accepting client: " + ex.Message);
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] data = new byte[256];
                int bytesRead;

                while ((bytesRead = stream.Read(data, 0, data.Length)) > 0)
                {
                    string message = Encoding.ASCII.GetString(data, 0, bytesRead);
                    LoggerUtility.LogInfo("Received message: " + message);

                    // Example response
                    string responseMessage = "Message received!";
                    byte[] responseData = Encoding.ASCII.GetBytes(responseMessage);
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                LoggerUtility.LogInfo("Error in client communication: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        public void StopListening()
        {
            try
            {
                listener.Stop();
                LoggerUtility.LogInfo("Listener stopped.");
            }
            catch (Exception ex)
            {
                LoggerUtility.LogInfo("Error stopping listener: " + ex.Message);
            }
        }
    }
}
