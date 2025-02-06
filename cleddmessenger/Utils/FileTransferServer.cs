using System;
using System.IO;
using System.Net.Sockets;
using Neme.Utils;

namespace Neme.Utils
{
    internal class FileTransferServer
    {
        private readonly TcpListener listener;
        private readonly AesEncryption aesEncryption;

        public FileTransferServer(int port, string key)
        {
            listener = new TcpListener(System.Net.IPAddress.Any, port);
            listener.Start();
            aesEncryption = new AesEncryption(key); // Use your existing encryption class
        }

        public void StartReceiving()
        {
            TcpClient tcpClient = listener.AcceptTcpClient();
            NetworkStream stream = tcpClient.GetStream();

            // Read file metadata
            byte[] fileNameLengthBytes = new byte[4];
            stream.Read(fileNameLengthBytes, 0, 4);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);

            byte[] fileNameBytes = new byte[fileNameLength];
            stream.Read(fileNameBytes, 0, fileNameBytes.Length);
            string fileName = System.Text.Encoding.UTF8.GetString(fileNameBytes);

            byte[] fileLengthBytes = new byte[4];
            stream.Read(fileLengthBytes, 0, 4);
            int fileLength = BitConverter.ToInt32(fileLengthBytes, 0);

            // Read encrypted file data
            byte[] encryptedBytes = new byte[fileLength];
            stream.Read(encryptedBytes, 0, encryptedBytes.Length);
            string encryptedData = Convert.ToBase64String(encryptedBytes);

            // Decrypt the file data
            string decryptedData = aesEncryption.Decrypt(encryptedData);
            byte[] fileData = Convert.FromBase64String(decryptedData);

            // Save the file
            string savePath = Path.Combine("ReceivedFiles", fileName);
            Directory.CreateDirectory("ReceivedFiles");
            File.WriteAllBytes(savePath, fileData);

            Console.WriteLine($"File '{fileName}' received and saved to {savePath}.");
        }
    }
}
