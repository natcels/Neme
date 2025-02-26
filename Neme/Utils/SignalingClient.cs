using Microsoft.MixedReality.WebRTC;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Neme.Utils
{
    public class SignalingClient
    {
        private readonly ClientWebSocket _socket;
        private readonly Uri _serverUri;

        public event Action<string, string, string> OnSdpReceived; // sender, sdp, type
        public event Action<string, IceCandidate> OnIceCandidateReceived;
        public event Action<string, string> OnCallReceived;
        public event Action<string> OnCallAccepted;
        public event Action<string> OnCallRejected;
        public event Action<string> OnCallEnded;

        public SignalingClient(string serverUrl)
        {
            _socket = new ClientWebSocket();
            _serverUri = new Uri(serverUrl);
        }

        public async Task ConnectAsync()
        {
            await _socket.ConnectAsync(_serverUri, CancellationToken.None);
            Console.WriteLine("Connected to signaling server.");
            _ = Task.Run(ReceiveMessagesAsync);
        }



        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];

            while (_socket.State == WebSocketState.Open)
            {
                var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var data = JsonSerializer.Deserialize<SignalingMessage>(message);

                    if (data == null) continue;

                    switch (data.Type)
                    {
                        case "offer":
                        case "answer":
                            OnSdpReceived?.Invoke(data.SenderId, data.Sdp, data.Type);
                            break;
                        case "candidate":
                            OnIceCandidateReceived?.Invoke(data.SenderId, data.IceCandidate);
                            break;
                        case "call":
                            OnCallReceived?.Invoke(data.SenderId, data.Sdp);
                            break;
                        case "acceptCall":
                            OnCallAccepted?.Invoke(data.SenderId);
                            break;
                        case "rejectCall":
                            OnCallRejected?.Invoke(data.SenderId);
                            break;
                        case "endCall":
                            OnCallEnded?.Invoke(data.SenderId);
                            break;
                    }
                }
            }
        }

        public async Task SendSdp(string recipient, string sdp, string type)
        {
            var message = new SignalingMessage
            {
                Type = type,
                SenderId = "me",
                RecipientId = recipient,
                Sdp = sdp
            };

            var json = JsonSerializer.Serialize(message);
            await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendIceCandidate(string recipient, IceCandidate candidate)
        {
            var message = new SignalingMessage
            {
                Type = "candidate",
                SenderId = "me",
                RecipientId = recipient,
                IceCandidate = candidate
            };

            var json = JsonSerializer.Serialize(message);
            await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task StartCall(string recipient)
        {
            var message = new SignalingMessage
            {
                Type = "call",
                SenderId = "me",
                RecipientId = recipient
            };

            var json = JsonSerializer.Serialize(message);
            await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task AcceptCall(string recipient)
        {
            var message = new SignalingMessage
            {
                Type = "acceptCall",
                SenderId = "me",
                RecipientId = recipient
            };

            var json = JsonSerializer.Serialize(message);
            await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task RejectCall(string recipient)
        {
            var message = new SignalingMessage
            {
                Type = "rejectCall",
                SenderId = "me",
                RecipientId = recipient
            };

            var json = JsonSerializer.Serialize(message);
            await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task EndCall(string recipient)
        {
            var message = new SignalingMessage
            {
                Type = "endCall",
                SenderId = "me",
                RecipientId = recipient
            };

            var json = JsonSerializer.Serialize(message);
            await _socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
