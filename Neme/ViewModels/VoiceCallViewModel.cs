//using Neme.Utils;
//using System;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Microsoft.MixedReality.WebRTC;

//namespace Neme.ViewModels
//{
//    public class VoiceCallViewModel : BaseViewModel
//    {
//        private readonly SignalingClient _signalingClient;
//        private PeerConnection _peerConnection;
//        private LocalAudioTrack _localAudioTrack;
//        private RemoteAudioTrack _remoteAudioTrack;

//        public event Action<RemoteAudioTrack> OnRemoteAudioReceived;
//        public string CallParticipant { get; private set; }

//        public ICommand AcceptCallCommand { get; }
//        public ICommand RejectCallCommand { get; }
//        public ICommand EndCallCommand { get; }

//        public VoiceCallViewModel(SignalingClient signalingClient)
//        {
//            _signalingClient = signalingClient;
//            _signalingClient.OnCallReceived += HandleIncomingCall;
//            _signalingClient.OnSdpReceived += HandleSdpMessage;
//            _signalingClient.OnIceCandidateReceived += HandleIceCandidate;

//            //AcceptCallCommand = new RelayCommand(async () => await AcceptCall());
//            //RejectCallCommand = new RelayCommand(async () => await RejectCall());
//            //EndCallCommand = new RelayCommand(async () => await EndCall());
//        }

//        private async Task InitializeConnection()
//        {
//            if (_peerConnection != null)
//                return;

//            _peerConnection = new PeerConnection();

//            _peerConnection.IceCandidateReadytoSend += candidate =>
//            {
//                _signalingClient.SendIceCandidate(CallParticipant, candidate);
//            };

//            _peerConnection.AudioTrackAdded += track =>
//            {
//                if (track is RemoteAudioTrack audioTrack)
//                {
//                    _remoteAudioTrack = audioTrack;
//                    OnRemoteAudioReceived?.Invoke(audioTrack);
//                }
//            };

//            await _peerConnection.InitializeAsync();
//        }

//        public async Task StartVoiceCall(string recipient)
//        {
//            await InitializeConnection();
//            CallParticipant = recipient;

//            var microphoneSource = await DeviceAudioTrackSource.CreateAsync();
//            var audioTrackConfig = new LocalAudioTrackInitConfig { trackName = "microphone_track" };

//            _localAudioTrack = LocalAudioTrack.CreateFromSource(microphoneSource, audioTrackConfig);
//            var audioTransceiver = _peerConnection.AddTransceiver(MediaKind.Audio);
//            audioTransceiver.LocalAudioTrack = _localAudioTrack;
//            audioTransceiver.DesiredDirection = Transceiver.Direction.SendReceive;

//            var offer =  _peerConnection.CreateOffer();
//           // await _peerConnection.SetLocalDescriptionAsync(offer);
//            //await _signalingClient.SendSdp(CallParticipant, offer, "offer");

//            LoggerUtility.LogInfo("Sent voice call offer.");
//        }

//        private async void HandleSdpMessage(string sender, string sdp, string type)
//        {
//            if (sender != CallParticipant) return;

//            if (type == "offer")
//            {
//                await InitializeConnection();
//                await _peerConnection.SetRemoteDescriptionAsync(new SdpMessage { Content = sdp, Type = SdpMessageType.Offer });

//                var answer =  _peerConnection.CreateAnswer();
//                //await _peerConnection.SetLocalDescriptionAsy(answer);
//                //await _signalingClient.SendSdp(sender, answer.Content, "answer");
//            }
//            else if (type == "answer")
//            {
//                await _peerConnection.SetRemoteDescriptionAsync(new SdpMessage { Content = sdp, Type = SdpMessageType.Answer });
//            }
//        }

//        private void HandleIceCandidate(string sender, IceCandidate candidate)
//        {
//            if (sender == CallParticipant)
//            {
//                _peerConnection.AddIceCandidate(candidate);
//            }
//        }

//        private void HandleIncomingCall(string sender, string callType)
//        {
//            if (callType == "voice")
//            {
//                CallParticipant = sender;
//                LoggerUtility.LogInfo($"Incoming voice call from {sender}");
//            }
//        }

//        private async Task AcceptCall()
//        {
//            await StartVoiceCall(CallParticipant);
//            LoggerUtility.LogInfo($"Accepted voice call from {CallParticipant}");
//        }

//        private async Task RejectCall()
//        {
//            await _signalingClient.RejectCall(CallParticipant);
//            LoggerUtility.LogInfo($"Rejected voice call from {CallParticipant}");
//        }

//        private async Task EndCall()
//        {
//            await _signalingClient.EndCall(CallParticipant);
//            _localAudioTrack?.Dispose();
//            _peerConnection?.Close();
//            _peerConnection = null;
//            _localAudioTrack = null;
//            LoggerUtility.LogInfo($"Ended voice call with {CallParticipant}");
//        }
//    }
//}
