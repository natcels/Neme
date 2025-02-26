using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Neme.Utils
{
    internal class TcpClientHandler
    {
        private TcpClient client;
        private NetworkStream stream;

        public TcpClientHandler(string ipAddress, int port, string key)
        {
            Connect(ipAddress, port);
        }

        public void SendFile(string filePath, string receiver)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);

            using (NetworkStream stream = client.GetStream())
            {
                // Send metadata
                string metadata = $"{fileName}|{fileData.Length}";
                byte[] metadataBytes = Encoding.UTF8.GetBytes(metadata);
                stream.Write(metadataBytes, 0, metadataBytes.Length);

                // Send file data
                stream.Write(fileData, 0, fileData.Length);
            }
        }

        public void ReceiveFile(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // Extract metadata
            string metadata = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            string[] metadataParts = metadata.Split('|');
            string fileName = metadataParts[0];
            int fileSize = int.Parse(metadataParts[1]);

            // Save file
            string savePath = Path.Combine("ReceivedFiles", fileName);
            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                byte[] fileBuffer = new byte[8192];
                int totalBytesRead = 0;

                while (totalBytesRead < fileSize)
                {
                    bytesRead = stream.Read(fileBuffer, 0, fileBuffer.Length);
                    fs.Write(fileBuffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                }
            }
        }


        private void Connect(string ipAddress, int port)
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
            }
        }

        public async Task<string> ReceiveMessageAsync()
        {
            try
            {
                byte[] data = new byte[256];
                int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                return Encoding.ASCII.GetString(data, 0, bytesRead);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving message: " + ex.Message);
                return string.Empty;
            }
        }

        public async Task SendEncryptedMessageAsync(string plainText)
        {
            string encryptedMessage = AesEncryption.Encrypt(plainText);
            await SendMessageAsync(encryptedMessage);
        }

        public async Task<string> ReceiveDecryptedMessageAsync()
        {
            string encryptedMessage = await ReceiveMessageAsync();
            return AesEncryption.Decrypt(encryptedMessage);
        }

        public async Task ReconnectAsync(string ipAddress, int port)
        {
            try
            {
                client.Close();
                Connect(ipAddress, port);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reconnecting: " + ex.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error closing connection: " + ex.Message);
            }
        }
    }
}
