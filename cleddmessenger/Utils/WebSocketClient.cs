﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;


namespace cleddmessenger.Utils
{
    internal class WebSocketClient
    {
        private WebSocket _webSocket;
        public WebSocketClient()
        {
            _webSocket = new WebSocket("ws://127.0.0.1:8080/signaling");  // Replace with server IP
            _webSocket.OnMessage += (sender, e) =>
            {
                Console.WriteLine("Received: " + e.Data);
            };
        }
        public void Connect()
        {
            try
            {
                _webSocket.Connect();
                LoggerUtility.LogInfo("Websocket started");
            }
            catch (Exception ex) {
                LoggerUtility.LogError(ex);   
            }

           
        }

        public void SendMessage(string message)
        {
            _webSocket.Send(message);
            LoggerUtility.LogInfo($"message sent");
        }

        public void CloseConnection()
        {
            _webSocket.Close();
            LoggerUtility.LogInfo("Connection Closed");
        }

    }
}
