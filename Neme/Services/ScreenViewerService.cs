using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Neme.Services
{
    public class ScreenViewerService  
    {
        private UdpClient udpClient;
        private const int ScreenSharePort = 55055;
        private bool isReceiving = false;
        private System.Windows.Controls.Image screenDisplay; // WPF Image control for displaying frames
      //  private ImageFormat imageFormat;

        public ScreenViewerService(System.Windows.Controls.Image display)
        {
            screenDisplay = display;
            udpClient = new UdpClient(ScreenSharePort);
        }

        public async Task StartReceiving()
        {
            isReceiving = true;
            while (isReceiving)
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                byte[] receivedData = result.Buffer;
                Bitmap image = DecompressFrame(receivedData);
                UpdateUI(image);
            }
        }

        public void StopReceiving()
        {
            isReceiving = false;
        }

        private Bitmap DecompressFrame(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return new Bitmap(ms);
            }
        }

        private void UpdateUI(Bitmap image)
        {
            screenDisplay.Dispatcher.Invoke(() =>
            {
                using (MemoryStream ms = new MemoryStream())
                {

                    image.Save(ms, ImageFormat.Bmp);
                    ms.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    screenDisplay.Source = bitmapImage;
                }
            });
        }
    }
}