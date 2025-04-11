using Microsoft.MixedReality.WebRTC;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Neme.Utils
{
    public class SignalingServer
    {
        private readonly HttpListener _listener;
        private readonly ConcurrentDictionary<string, WebSocket> _clients = new();

        public SignalingServer(string url)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(url);
        }

        public async Task StartAsync()
        {
            _listener.Start();
            LoggerUtility.LogInfo("Signaling Server started...");

            while (true)
            {
                var context = await _listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var webSocketContext = await context.AcceptWebSocketAsync(null);
                    var connectionId = Guid.NewGuid().ToString();
                    LoggerUtility.LogInfo($"Client connected: {connectionId}");

                    _clients[connectionId] = webSocketContext.WebSocket;
                    _ = HandleClientAsync(connectionId, webSocketContext.WebSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task HandleClientAsync(string clientId, WebSocket socket)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        LoggerUtility.LogInfo($"Client {clientId} disconnected.");
                        _clients.TryRemove(clientId, out _);
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        return;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var data = JsonSerializer.Deserialize<SignalingMessage>(message);

                    if (data != null && _clients.TryGetValue(data.RecipientId, out var recipientSocket))
                    {
                        await recipientSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerUtility.LogInfo($"Error with client {clientId}: {ex.Message}");
                _clients.TryRemove(clientId, out _);
            }
        }
    }

    public class SignalingMessage
    {
        public string Type { get; set; } // "offer", "answer", "candidate", "call", etc.
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string Sdp { get; set; }
        public IceCandidate IceCandidate { get; set; }
    }
}
