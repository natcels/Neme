using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.MixedReality.WebRTC;
using static Microsoft.MixedReality.WebRTC.PeerConnection;

namespace Neme.Utils
{
    public class WebRtcManager
    {
        private PeerConnection _peerConnection;

        public async Task InitializeAsync()
        {
            if (_peerConnection != null)
            {
                LoggerUtility.LogInfo("PeerConnection already initialized.");
                return;
            }

            _peerConnection = new PeerConnection();

            // Subscribe to events **after** initializing
            _peerConnection.Connected += () => LoggerUtility.LogInfo("✅ Peer connected!");

            _peerConnection.LocalSdpReadytoSend += OnLocalSdpReadytoSend;

            _peerConnection.IceCandidateReadytoSend += new IceCandidateReadytoSendDelegate(OnIceCandidateReady); ;

            var config = new PeerConnectionConfiguration
            {
                IceServers = new List<IceServer>
                {
                    new IceServer { Urls = new List<string> { "stun:stun.l.google.com:19302" } }
                }
            };

            await _peerConnection.InitializeAsync(config);
            LoggerUtility.LogInfo("✅ WebRTC Initialized");
        }

        private void OnIceCandidateReady(Microsoft.MixedReality.WebRTC.IceCandidate candidate)
        {
            LoggerUtility.LogInfo($"ICE Candidate: {candidate.Content}");
            SendCandidateToPeer(candidate.Content, candidate.SdpMid, candidate.SdpMlineIndex);
        }

        private void OnLocalSdpReadytoSend(SdpMessage sdp)
        {
            LoggerUtility.LogInfo($"SDP Message: {sdp.Content}");
            SendSdpToPeer(sdp);
        }

        public async Task StartConnectionAsync()
        {
            bool success = _peerConnection.CreateOffer();
            if (!success)
            {
                LoggerUtility.LogError("Failed to create WebRTC offer.");
            }
        }

        private void SendSdpToPeer(SdpMessage sdp)
        {
            // Use your signaling mechanism to send the SDP message to the other peer
            LoggerUtility.LogInfo($"Sending SDP {sdp.Type}: {sdp.Content}");
        }

        private void SendCandidateToPeer(string candidate, string sdpMid, int sdpMLineIndex)
        {
            // Send ICE candidate using your signaling mechanism
            LoggerUtility.LogInfo($"Sending ICE Candidate: {candidate}");
        }

        public async Task ReceiveOfferAsync(string sdp)
        {
            var remoteDesc = new SdpMessage { Type = SdpMessageType.Offer, Content = sdp };
            await _peerConnection.SetRemoteDescriptionAsync(remoteDesc);

            bool success = _peerConnection.CreateAnswer();
            if (!success)
            {
                LoggerUtility.LogError("Failed to create WebRTC answer.");
            }
        }
    }
}
