using System;
using System.Collections.Generic;
using Neme.Models;
using Neme.Utils;

namespace Neme.Services
{
    public class ChatMessageService
    {
        private static readonly Dictionary<string, string> PeerPublicKeys = new Dictionary<string, string>();

        /// <summary>
        /// Called when the app starts or a peer joins.
        /// Sends this peer's public key to all others.
        /// </summary>
        public void BroadcastPublicKey(string senderName)
        {
            var publicKey = RSAKeyManager.GetPublicKey();
            var keyMessage = new ChatMessage
            {
                SenderName = senderName,
                ReceiverName = "ALL",
                Content = publicKey, // 🔹 Send the public key as a message
                Type = MessageType.KeyExchange,
                Timestamp = DateTime.Now
            };

            SendMessage(keyMessage);
        }

        /// <summary>
        /// Handles incoming messages, including public key exchange.
        /// </summary>
        public void ReceiveMessage(ChatMessage message)
        {
            if (message.Type == MessageType.KeyExchange)
            {
                // 🔹 Store the sender's public key
                if (!PeerPublicKeys.ContainsKey(message.SenderName))
                {
                    PeerPublicKeys[message.SenderName] = message.Content;
                    Console.WriteLine($"🔑 Received public key from {message.SenderName}");
                }
            }
            else
            {
                // Process normal messages (text, file, etc.)
                Console.WriteLine($"📩 {message.SenderName}: {message.Content}");
            }
        }

        /// <summary>
        /// Encrypts and sends a message to a peer.
        /// </summary>
        public void SendMessage(ChatMessage message)
        {
            if (message.Type == MessageType.Text && PeerPublicKeys.ContainsKey(message.ReceiverName))
            {
                message.EncryptedContent = RSAKeyManager.EncryptWithPublicKey(message.Content, PeerPublicKeys[message.ReceiverName]);
            }

            // Send the message over the network (to be implemented)
            Console.WriteLine($"📤 Sending message to {message.ReceiverName}");
        }
    }
}
