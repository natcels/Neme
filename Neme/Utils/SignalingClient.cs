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
        private readonly ClientWebSocket _webSocket = new ClientWebSocket();
        private readonly Uri _serverUri;
        private readonly string _clientId;
        private PeerConnection _peerConnection;

        public SignalingClient(string serverUrl, string clientId)
        {
            _serverUri = new Uri(serverUrl);
            _clientId = clientId;
        }

        public async Task ConnectAsync()
        {
            await _webSocket.ConnectAsync(_serverUri, CancellationToken.None);
            LoggerUtility.LogInfo("Connected to signaling server.");
            _ = Task.Run(ReceiveMessagesAsync);
        }

        public async Task SendMessageAsync(SignalingMessage message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var bytes = Encoding.UTF8.GetBytes(jsonMessage);
            var buffer = new ArraySegment<byte>(bytes);

            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        LoggerUtility.LogInfo("Disconnected from signaling server.");
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        return;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var signalingMessage = JsonSerializer.Deserialize<SignalingMessage>(message);

                    // Handle the signaling message (e.g., process offers, answers, or ICE candidates)
                    HandleSignalingMessage(signalingMessage);
                }
            }
            catch (Exception ex)
            {
                LoggerUtility.LogInfo($"Error receiving messages: {ex.Message}");
            }
        }

        private async void HandleSignalingMessage(SignalingMessage message)
        {
            switch (message.Type.ToLower())
            {
                case "offer":
                    await HandleOfferAsync(message);
                    break;

                case "answer":
                    await HandleAnswerAsync(message);
                    break;

                case "candidate":
                    HandleIceCandidate(message);
                    break;

                case "leave":
                    HandleLeave();
                    break;

                default:
                    LoggerUtility.LogError($"Unknown signaling message type: {message.Type}");
                    break;
            }
        }

        private async Task HandleOfferAsync(SignalingMessage message)
        {
            LoggerUtility.LogInfo("Received SDP offer.");
            var remoteDesc = new SdpMessage { Type = SdpMessageType.Offer, Content = message.Sdp };
            await _peerConnection.SetRemoteDescriptionAsync(remoteDesc);

            bool success = _peerConnection.CreateAnswer();
            if (!success)
            {
                LoggerUtility.LogError("Failed to create WebRTC answer.");
            }
        }

        private async Task HandleAnswerAsync(SignalingMessage message)
        {
            LoggerUtility.LogInfo("Received SDP answer.");
            var remoteDesc = new SdpMessage { Type = SdpMessageType.Answer, Content = message.Sdp };
            await _peerConnection.SetRemoteDescriptionAsync(remoteDesc);
        }

        private void HandleIceCandidate(SignalingMessage message)
        {
            LoggerUtility.LogInfo("Received ICE candidate.");
            var candidate = new IceCandidate
            {
                Content = message.IceCandidate.Candidate,
                SdpMid = message.IceCandidate.SdpMid,
                SdpMlineIndex = message.IceCandidate.SdpMLineIndex
            };
            _peerConnection.AddIceCandidate(candidate);
        }

        private void HandleLeave()
        {
            LoggerUtility.LogInfo("Remote peer has left the session.");
        }
    }

    public class IceCandidate : Microsoft.MixedReality.WebRTC.IceCandidate
    {
        public string Candidate { get; set; }
        public string SdpMid { get; set; }
        public int SdpMLineIndex { get; set; }
    }
}
