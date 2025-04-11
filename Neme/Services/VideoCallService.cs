using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Neme.Services
{
    public class VideoCallService
    {
        private const int VideoCallPort = 55052; // Dedicated Video Call Port
        private readonly UdpClient udpClient;
        private bool isRunning;
        private Thread receiveThread;
        public event Action<Image> OnFrameReceived;
        public event Action<string> OnCallError; // For error handling

        public VideoCallService()
        {
            udpClient = new UdpClient();
        }

        /// <summary>
        /// Starts video transmission.
        /// </summary>
        public void StartVideoCall(string peerAddress)
        {
            try
            {
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, VideoCallPort));
                isRunning = true;

                receiveThread = new Thread(ReceiveVideoFrames);
                receiveThread.Start();

                // Start capturing and sending video frames
                Thread sendThread = new Thread(() => SendVideoFrames(peerAddress));
                sendThread.Start();
            }
            catch (Exception ex)
            {
                OnCallError?.Invoke($"Video call error: {ex.Message}");
            }
        }

        /// <summary>
        /// Captures and sends video frames to the peer.
        /// </summary>
        private void SendVideoFrames(string peerAddress)
        {
            try
            {
                IPEndPoint peerEndPoint = new IPEndPoint(IPAddress.Parse(peerAddress), VideoCallPort);
                while (isRunning)
                {
                    using (Bitmap frame = CaptureScreen())
                    using (MemoryStream ms = new MemoryStream())
                    {
                        frame.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] frameData = ms.ToArray();
                        udpClient.Send(frameData, frameData.Length, peerEndPoint);
                    }
                    Thread.Sleep(100); // Control frame rate
                }
            }
            catch (Exception ex)
            {
                OnCallError?.Invoke($"Error sending video: {ex.Message}");
            }
        }

        /// <summary>
        /// Receives and processes incoming video frames.
        /// </summary>
        private void ReceiveVideoFrames()
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, VideoCallPort);
                while (isRunning)
                {
                    byte[] receivedData = udpClient.Receive(ref remoteEP);
                    using (MemoryStream ms = new MemoryStream(receivedData))
                    {
                        Image receivedFrame = Image.FromStream(ms);
                        OnFrameReceived?.Invoke(receivedFrame);
                    }
                }
            }
            catch (SocketException ex)
            {
                OnCallError?.Invoke($"Socket error in video call: {ex.Message}");
            }
            catch (Exception ex)
            {
                OnCallError?.Invoke($"General error in video call: {ex.Message}");
            }
        }

        /// <summary>
        /// Captures the current screen as a Bitmap.
        /// </summary>
        private Bitmap CaptureScreen()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            }
            return bitmap;
        }

        /// <summary>
        /// Stops the video call and releases resources.
        /// </summary>
        public void StopVideoCall()
        {
            isRunning = false;
            udpClient.Close();
        }
    }
}
