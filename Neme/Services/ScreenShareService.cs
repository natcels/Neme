using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Neme.Utils;

namespace Neme.Services
{
    public class ScreenShareService
    {
        private UdpClient udpClient;
        private const int ScreenSharePort = 55055;
        private string targetIP;
        private bool isSharing = false;

        public ScreenShareService(string targetIP)
        {
            this.targetIP = targetIP;
            udpClient = new UdpClient();
        }

        public async Task StartSharing()
        {
            isSharing = true;
            while (isSharing)
            {
                Bitmap screenFrame = CaptureScreen();
                byte[] compressedFrame = CompressFrame(screenFrame);
                await SendFrame(compressedFrame);
                await Task.Delay(100); // Limit frame rate to avoid high CPU usage
            }
        }

        public void StopSharing()
        {
            isSharing = false;
        }

        private Bitmap CaptureScreen()
        {
            Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
            return bitmap;
        }

        private byte[] CompressFrame(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg); // Use JPEG for compression
                return ms.ToArray();
            }
        }

        private async Task SendFrame(byte[] frameData)
        {
            try
            {
                await udpClient.SendAsync(frameData, frameData.Length, new IPEndPoint(IPAddress.Parse(targetIP), ScreenSharePort));
            }
            catch (Exception ex)
            {
                LoggerUtility.LogInfo("Screen Share Error: " + ex.Message);
            }
        }
    }
}
