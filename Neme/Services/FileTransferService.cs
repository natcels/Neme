using Neme.Utils;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Neme.Services
{
    public class FileTransferService
    {
        private const int BufferSize = 4096;
        private const int FileTransferPort = 55054;
        private const int MaxRetries = 3;

      
        public async Task StartListening()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, FileTransferPort);
            listener.Start();
            LoggerUtility.LogInfo("📂 File Transfer Service started. Listening for incoming files...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleIncomingFile(client)); // Handle each file in a new task
            }
        }

        private async Task HandleIncomingFile(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            using (BinaryReader reader = new BinaryReader(stream))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                string fileName = reader.ReadString();
                long fileSize = reader.ReadInt64();
                LoggerUtility.LogInfo($"📥 Receiving file: {fileName} ({fileSize} bytes)");

                string savePath = Path.Combine("ReceivedFiles", fileName);
                Directory.CreateDirectory("ReceivedFiles");

                long existingSize = 0;

                // Check if a partial file exists
                if (File.Exists(savePath))
                {
                    existingSize = new FileInfo(savePath).Length;
                    if (existingSize > fileSize || existingSize == 0) // Corrupted file case
                    {
                        File.Delete(savePath);
                        existingSize = 0;
                    }
                }

                // Inform sender where to resume
                writer.Write(existingSize);

                long bytesReceived = existingSize;
                byte[] buffer = new byte[BufferSize];
                int retries = 0;

                using (FileStream fs = new FileStream(savePath, FileMode.Append, FileAccess.Write))
                {
                    int bytesRead;
                    while (bytesReceived < fileSize && (bytesRead = await stream.ReadAsync(buffer, 0, BufferSize)) > 0)
                    {
                        while (retries < MaxRetries)
                        {
                            try
                            {
                                fs.Write(buffer, 0, bytesRead);
                                bytesReceived += bytesRead;
                                retries = 0; // Reset retry count
                                break;
                            }
                            catch
                            {
                                retries++;
                                if (retries >= MaxRetries)
                                {
                                    LoggerUtility.LogError("⚠️ Transfer failed. Retrying...");
                                    await Task.Delay(2000);
                                }
                            }
                        }

                        if (retries >= MaxRetries)
                        {
                            LoggerUtility.LogError("❌ Transfer failed. Please restart.");
                            return;
                        }

                        // Progress reporting
                        double progress = (double)bytesReceived / fileSize * 100;
                        LoggerUtility.LogInfo($"📊 Progress: {progress:F2}%");
                    }
                }

                // Decompress & save file
                byte[] compressedData = await File.ReadAllBytesAsync(savePath);
                byte[] decompressedData = FileCompressor.DecompressFile(compressedData);
                await File.WriteAllBytesAsync(savePath, decompressedData);

                LoggerUtility.LogInfo($"✅ File {fileName} received successfully!");
            }
        }
 

        public async Task SendFile(string filePath, string targetIP, ProgressBar progressBar, TextBlock progressText, CancellationToken cancellationToken)
        {
            if (!File.Exists(filePath)) return;

            FileInfo fileInfo = new FileInfo(filePath);
            byte[] compressedData = FileCompressor.CompressFile(filePath);
            long totalSize = compressedData.Length;
            long bytesSent = 0;
            int retries = 0;

            using (TcpClient client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(targetIP, FileTransferPort);
                    using (NetworkStream stream = client.GetStream())
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        // Send file metadata
                        writer.Write(fileInfo.Name);
                        writer.Write(totalSize);

                        // Receive resume position
                        long resumePosition = reader.ReadInt64();
                        bytesSent = resumePosition;

                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            fs.Seek(resumePosition, SeekOrigin.Begin);

                            byte[] buffer = new byte[BufferSize];
                            int bytesRead;

                            while ((bytesRead = fs.Read(buffer, 0, BufferSize)) > 0)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    progressText.Dispatcher.Invoke(() => progressText.Text = "❌ Transfer Cancelled");
                                    return;
                                }

                                // Retry mechanism
                                while (retries < MaxRetries)
                                {
                                    try
                                    {
                                        await stream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                                        bytesSent += bytesRead;
                                        retries = 0; // Reset retry count on success

                                        // UI Progress Update
                                        double progress = (double)bytesSent / totalSize * 100;
                                        progressBar.Dispatcher.Invoke(() => progressBar.Value = progress);
                                        progressText.Dispatcher.Invoke(() => progressText.Text = $"{progress:F2}% Completed");

                                        break;
                                    }
                                    catch
                                    {
                                        retries++;
                                        if (retries >= MaxRetries)
                                        {
                                            progressText.Dispatcher.Invoke(() => progressText.Text = "❌ Transfer Failed. Retrying...");
                                            await Task.Delay(2000, cancellationToken);
                                        }
                                    }
                                }

                                if (retries >= MaxRetries)
                                {
                                    progressText.Dispatcher.Invoke(() => progressText.Text = "⚠️ Transfer Failed. Please restart.");
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    progressText.Dispatcher.Invoke(() => progressText.Text = "❌ Error: " + ex.Message);
                }
            }
        }


    }
}
