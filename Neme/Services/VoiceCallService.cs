using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NAudio.Wave;

namespace Neme.Services
{
    public class VoiceCallService
    {
        private const int VoiceCallPort = 55051; // Dedicated Voice Call Port
        private readonly UdpClient udpClient;
        private bool isRunning;
        private WaveInEvent waveIn;
        private WaveOut waveOut;
        private BufferedWaveProvider bufferedWaveProvider;
        private Thread receiveThread;

        public event Action<string> OnCallError; // For error handling

        public VoiceCallService()
        {
            udpClient = new UdpClient();
        }

        /// <summary>
        /// Starts the voice call by initializing recording and listening.
        /// </summary>
        public void StartVoiceCall(string peerAddress)
        {
            try
            {
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, VoiceCallPort));

                // Setup Audio Input (Microphone)
                waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(8000, 16, 1)
                };
                waveIn.DataAvailable += SendVoiceData;
                waveIn.StartRecording();

                // Setup Audio Output (Speaker)
                bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
                waveOut = new WaveOut();
                waveOut.Init(bufferedWaveProvider);
                waveOut.Play();

                isRunning = true;
                receiveThread = new Thread(() => ReceiveVoiceData(peerAddress));
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                OnCallError?.Invoke($"Voice call error: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends captured audio data to the peer.
        /// </summary>
        private void SendVoiceData(object sender, WaveInEventArgs e)
        {
            try
            {
                udpClient.Send(e.Buffer, e.Buffer.Length);
            }
            catch (Exception ex)
            {
                OnCallError?.Invoke($"Error sending voice data: {ex.Message}");
            }
        }

        /// <summary>
        /// Receives incoming voice data and plays it.
        /// </summary>
        private void ReceiveVoiceData(string peerAddress)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, VoiceCallPort);
                while (isRunning)
                {
                    byte[] receivedData = udpClient.Receive(ref remoteEP);
                    bufferedWaveProvider.AddSamples(receivedData, 0, receivedData.Length);
                }
            }
            catch (SocketException ex)
            {
                OnCallError?.Invoke($"Socket error in voice call: {ex.Message}");
            }
            catch (Exception ex)
            {
                OnCallError?.Invoke($"General error in voice call: {ex.Message}");
            }
        }

        /// <summary>
        /// Stops the voice call and releases resources.
        /// </summary>
        public void StopVoiceCall()
        {
            isRunning = false;
            waveIn?.StopRecording();
            waveIn?.Dispose();
            waveOut?.Stop();
            waveOut?.Dispose();
            udpClient.Close();
        }
    }
}
