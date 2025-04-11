using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Neme.Models;
using Neme.Utils;

namespace Neme.Services
{
    public class ChatMessageService
    {
        private static readonly Dictionary<string, string> PeerPublicKeys = new Dictionary<string, string>();
        private const int ChatPort = 55051; // Port for chat messages
        private readonly Dictionary<string, List<ChatMessage>> messageStore = new Dictionary<string, List<ChatMessage>>();
        private readonly string _localPeerName;
        private readonly List<Peer> _peerList = new List<Peer>(); // Store peer IPs

        // Event for notifying UI about new messages
        public event Action<ChatMessage> OnMessageReceived;
        public event Action<string> OnMessageRead; // Notifies when a message is read

        
        public string LocalPeerName => _localPeerName; // ✅ Expose LocalPeerName

        
        public ChatMessageService()
        {
            _localPeerName = Environment.MachineName;
        }

        /// <summary>
        /// Sends an encrypted message directly to a specific peer.
        /// </summary>
        public async Task SendMessage(ChatMessage message, Peer peer)
        {
            if (!PeerPublicKeys.ContainsKey(peer.Address))
            {
                LoggerUtility.LogError($"❌ Cannot send message. No public key for {peer.Address}");
                return;
            }

            string publicKey = PeerPublicKeys[peer.Address];
            string jsonMessage = JsonSerializer.Serialize(message);
            string encryptedMessage = RSAKeyManager.EncryptWithPublicKey(jsonMessage, publicKey);
            string messageId = Guid.NewGuid().ToString(); // Unique message ID for tracking

            string finalMessage = $"MSG|{messageId}|{encryptedMessage}";

            StoreMessage(message, messageId); // Store before sending

            using (TcpClient client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(peer.Address, ChatPort);
                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    {
                        await writer.WriteLineAsync(finalMessage);
                    }
                }
                catch (Exception ex)
                {
                    LoggerUtility.LogError($"❌ Error sending message: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Starts listening for incoming messages.
        /// </summary>
        public async Task StartListening()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, ChatPort);
            listener.Start();
            LoggerUtility.LogInfo("💬 Chat Service started. Listening for messages...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleIncomingMessage(client));
            }
        }

        /// <summary>
        /// Processes incoming messages and acknowledgments.
        /// </summary>
        private async Task HandleIncomingMessage(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string receivedMessage = await reader.ReadLineAsync();
                if (receivedMessage == null) return;

                string[] parts = receivedMessage.Split('|');
                if (parts.Length < 3) return;

                string messageType = parts[0];
                string senderName = parts[1];

                if (messageType == "MSG")
                {
                    string messageId = parts[2];
                    string encryptedText = parts[3];

                    string decryptedMessage = RSAKeyManager.DecryptWithPrivateKey(encryptedText);
                    var chatMessage = JsonSerializer.Deserialize<ChatMessage>(decryptedMessage);
                    LoggerUtility.LogInfo($"📩 Message from {senderName}: {chatMessage.Content}");

                    // Notify UI about the new message
                    OnMessageReceived?.Invoke(chatMessage);

                    // Send acknowledgment
                    await SendAcknowledgment(senderName, messageId);
                }
                else if (messageType == "ACK")
                {
                    string messageId = parts[2];
                    LoggerUtility.LogInfo($"✅ Message {messageId} successfully received by {senderName}");
                    RemoveMessage(senderName, messageId);
                }
                else if (messageType == "READ")
                {
                    string messageId = parts[2];
                    LoggerUtility.LogInfo($"👀 Message {messageId} read by {senderName}");

                    // Notify UI about read receipt
                    OnMessageRead?.Invoke(messageId);
                }
            }
        }

        /// <summary>
        /// Sends an acknowledgment message back to the sender.
        /// </summary>
        private async Task SendAcknowledgment(string recipientIP, string messageId)
        {
            string ackMessage = $"ACK|{recipientIP}|{messageId}";

            using (TcpClient client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(recipientIP, ChatPort);
                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    {
                        await writer.WriteLineAsync(ackMessage);
                    }
                }
                catch (Exception ex)
                {
                    LoggerUtility.LogError($"❌ Error sending acknowledgment: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Sends a read receipt to the sender.
        /// </summary>
        public async Task SendReadReceipt(string senderIP, string messageId)
        {
            string readMessage = $"READ|{_localPeerName}|{messageId}";

            using (TcpClient client = new TcpClient())
            {
                try
                {
                    await client.ConnectAsync(senderIP, ChatPort);
                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    {
                        await writer.WriteLineAsync(readMessage);
                    }
                }
                catch (Exception ex)
                {
                    LoggerUtility.LogError($"❌ Error sending read receipt: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Stores a message before sending (for retries if needed).
        /// </summary>
        private void StoreMessage(ChatMessage message, string messageId)
        {
            if (!messageStore.ContainsKey(message.ReceiverName))
            {
                messageStore[message.ReceiverName] = new List<ChatMessage>();
            }

            messageStore[message.ReceiverName].Add(new ChatMessage
            {
                SenderName = message.SenderName,
                Content = message.Content,
                Id = messageId,
                Timestamp = DateTime.UtcNow
            });

            LoggerUtility.LogInfo($"💾 Stored message {messageId} for {message.ReceiverName}");
        }

        /// <summary>
        /// Removes a message after successful delivery.
        /// </summary>
        private void RemoveMessage(string recipientIP, string messageId)
        {
            if (messageStore.ContainsKey(recipientIP))
            {
                messageStore[recipientIP].RemoveAll(msg => msg.Id == messageId);
                LoggerUtility.LogInfo($"🗑️ Removed stored message {messageId}");
            }
        }

        public void BroadcastKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                LoggerUtility.LogError("Encryption key is empty. Cannot broadcast.");
                return;
            }

            var keyMessage = new ChatMessage
            {
                SenderName = _localPeerName,
                Content = key,
                Type = MessageType.KeyExchange
            };

            foreach (var peer in _peerList)
            {
                SendMessage(keyMessage, peer);
            }

            LoggerUtility.LogInfo($"Broadcasted encryption key: {key}");
        }

        public async Task SendFile(string filePath, Peer peer)
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();
            string[] allowedExtensions = { ".docx", ".pdf", ".xlsx", ".jpg", ".png", ".gif", ".mp3", ".mp4" };

            if (!allowedExtensions.Contains(fileExtension))
            {
                LoggerUtility.LogError("❌ Attempted to send an unsupported file type.");
                return;
            }

            LoggerUtility.LogInfo($"📤 Sending file {filePath} to {peer.Name}...");

        }
    }
}
