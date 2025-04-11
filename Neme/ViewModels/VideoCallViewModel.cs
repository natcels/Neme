//using System;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Microsoft.MixedReality.WebRTC;
//using Neme.Utils;
//using Neme.Views;

//namespace Neme.ViewModels
//{
//    public class VideoCallViewModel : BaseViewModel
//    {
//        private readonly SignalingClient _signalingClient;
//        //private PeerConnection _peerConnection;
//        //private LocalVideoTrack _localVideoTrack;
//        //private RemoteVideoTrack _remoteVideoTrack;

//        //public event Action<RemoteVideoTrack> OnRemoteVideoReceived;
//        //public string CallParticipant { get; private set; }

//        public ICommand AcceptCallCommand { get; }
//        public ICommand RejectCallCommand { get; }
//        public ICommand EndCallCommand { get; }

//        public VideoCallViewModel(SignalingClient signalingClient)
//        {
//            _signalingClient = signalingClient;
//            _signalingClient.OnCallReceived += HandleIncomingCall;
//            _signalingClient.OnSdpReceived += HandleSdpMessage;
//            _signalingClient.OnIceCandidateReceived += HandleIceCandidate;

//            InitializeConnection();
//            AcceptCallCommand = new RelayCommand(AcceptCall);
//            RejectCallCommand = new RelayCommand(RejectCall);
//            EndCallCommand = new RelayCommand(EndCall);
//        }

//        private async void InitializeConnection()
//        {
//            _peerConnection = new PeerConnection();

//            _peerConnection.IceCandidateReadytoSend += (candidate) =>
//            {
//                _signalingClient.SendIceCandidate(CallParticipant, candidate);
//            };

//            _peerConnection.VideoTrackAdded += (track) =>
//            {
//                if (track is RemoteVideoTrack videoTrack)
//                {
//                    _remoteVideoTrack = videoTrack;
//                    OnRemoteVideoReceived?.Invoke(videoTrack);
//                }
//            };

//            await _peerConnection.InitializeAsync();
//        }

        

//        private async void HandleSdpMessage(string sender, string sdp, string type)
//        {
//            if (sender != CallParticipant) return;

//            if (type == "offer")
//            {
//                await _peerConnection.SetRemoteDescriptionAsync(new SdpMessage { Content = sdp, Type = SdpMessageType.Offer });

//                var answer =  _peerConnection.CreateAnswer();
//                //await _peerConnection.SetLocalDescription(answer);
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
//            if (callType == "video")
//            {
//                CallParticipant = sender;
//                LoggerUtility.LogInfo($"Incoming video call from {sender}");
//                // UI logic for accepting/rejecting the call
//                VideoCallWindow videoCallWindow = new VideoCallWindow();    
//                videoCallWindow.ShowDialog();


//            }
//        }

//        private void AcceptCall()
//        {
//            _signalingClient.AcceptCall(CallParticipant);
           
//            StartVideoCall(CallParticipant);
//            LoggerUtility.LogInfo($"Accepted video call from {CallParticipant}");
//        }

//        private void RejectCall()
//        {
//            _signalingClient.RejectCall(CallParticipant);
//            LoggerUtility.LogInfo($"Rejected video call from {CallParticipant}");
//        }

//        private void EndCall()
//        {
//            _signalingClient.EndCall(CallParticipant);
//            _localVideoTrack?.Dispose();
//            _peerConnection?.Close();
//            LoggerUtility.LogInfo($"Ended video call with {CallParticipant}");
//        }

//        public async Task StartVideoCall(string recipient)
//        {
//            if (_peerConnection == null)
//                 InitializeConnection();

//            CallParticipant = recipient;
//            var webcamSource = await DeviceVideoTrackSource.CreateAsync();
//            var videoTrackConfig = new LocalVideoTrackInitConfig { trackName = "webcam_track" };

//            _localVideoTrack = LocalVideoTrack.CreateFromSource(webcamSource, videoTrackConfig);

//            var videoTransceiver = _peerConnection.AddTransceiver(MediaKind.Video);
//            videoTransceiver.LocalVideoTrack = _localVideoTrack;
//            videoTransceiver.DesiredDirection = Transceiver.Direction.SendReceive;

//           // var offer = await _peerConnection.CreateOfferAsync();
//            var offer =  _peerConnection.CreateOffer();
//           // await _peerConnection.SetLocalDescriptionAsync(offer);
//          //  await _peerConnection.SetRemoteDescriptionAsync(offer);
//            await _signalingClient.SendSdp(CallParticipant, offer, "offer");

//            LoggerUtility.LogInfo("Sent video call offer.");
//        }

//    }
//}
