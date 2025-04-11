using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net;

namespace Neme.Utils
{
    internal class WebSocketServe
    {
        private HttpListener _httpListener;
        public void Start()
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://localhost:8080/");

            _httpListener.Start();
            LoggerUtility.LogInfo("Server started at ws://localhost:8080/");

            while (true)
            {
                var context = _httpListener.GetContext();
                if (context.Request.IsWebSocketRequest)
                {
                    ProcessWebSocketRequest(context);
                }
            }
        }

        private async void ProcessWebSocketRequest(HttpListenerContext context)
        {
            WebSocket webSocket = (await context.AcceptWebSocketAsync(null)).WebSocket;

            while (webSocket.State == WebSocketState.Open)
            {
                var buffer = new byte[1024];
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    LoggerUtility.LogInfo("Received: " + message);

                    // Broadcast message to connected clients
                    // In a real scenario, manage multiple clients
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("Echo: " + message)), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public void Stop()
        {
            _httpListener.Stop();
        }

    }
}
