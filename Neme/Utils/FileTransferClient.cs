using System;
using System.IO;
using System.Net.Sockets;
using Neme.Utils;

namespace Neme.Utils
{
    internal class FileTransferClient
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream stream;

        public FileTransferClient(string ipAddress, int port = 33456, string key = null)
        {
            key ??= Environment.MachineName;
            tcpClient = new TcpClient(ipAddress, port);
            stream = tcpClient.GetStream();
        }

        public void SendFile(string filePath)
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                string fileName = Path.GetFileName(filePath);

                // Encrypt file data using AesEncryption
                string encryptedData = AesEncryption.Encrypt(Convert.ToBase64String(fileData));
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

                // Send file metadata and encrypted data
                SendFileMetadata(fileName, encryptedBytes.Length);
                stream.Write(encryptedBytes, 0, encryptedBytes.Length);

                LoggerUtility.LogInfo($"File '{fileName}' sent successfully.");
            }
            catch (Exception ex)
            {
                LoggerUtility.LogInfo($"Error sending file: {ex.Message}");
            }
            finally
            {
                CloseConnection();
            }
        }

        private void SendFileMetadata(string fileName, int dataLength)
        {
            byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
            byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);
            stream.Write(fileNameLengthBytes, 0, fileNameLengthBytes.Length); // File name length
            stream.Write(fileNameBytes, 0, fileNameBytes.Length); // File name
            byte[] fileLengthBytes = BitConverter.GetBytes(dataLength);
            stream.Write(fileLengthBytes, 0, fileLengthBytes.Length); // File data length
        }

        private void CloseConnection()
        {
            try
            {
                stream?.Close();
                tcpClient?.Close();
            }
            catch (Exception ex)
            {
                LoggerUtility.LogInfo($"Error closing connection: {ex.Message}");
            }
        }
    }
}
