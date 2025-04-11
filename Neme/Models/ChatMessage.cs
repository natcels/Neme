using Neme.Utils;
using System;
using System.ComponentModel;

namespace Neme.Models
{
    public class ChatMessage : INotifyPropertyChanged
    {

        private MessageStatus _deliveryStatus;
        private string _encryptedContent;

        public string Id { get; set; }

        public string EncryptedContent
        {
            get => _encryptedContent;
            set
            {
                if (_encryptedContent != value)
                {
                    _encryptedContent = value;
                    OnPropertyChanged(nameof(EncryptedContent));
                    OnPropertyChanged(nameof(Content)); // Ensure UI updates decrypted content too
                }
            }
        }

        public string Content { get; set; } // Automatically decrypt

        public bool IsSentByCurrentUser { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string SenderName { get; set; }
        public string SenderIP { get; set; }
        public string ReceiverName { get; set; }
        public string AvatarImage { get; set; }
        public bool IsRead { get; set; } = false;

        public MessageStatus DeliveryStatus
        {
            get => _deliveryStatus;
            set
            {
                if (_deliveryStatus != value)
                {
                    _deliveryStatus = value;
                    OnPropertyChanged(nameof(DeliveryStatus)); // Notify UI when status changes
                }
            }
        }

        public string FilePath { get; set; } // File location
        public MessageType Type { get; set; } // Text, Image, Video, File, etc.
        public string FileName { get; set; }  // File Name for displaying in UI

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Enum to represent the delivery status of a message.
    /// </summary>
    public enum MessageStatus
    {
        Sent,
        Delivered,
        Read,
        Failed
    }

    /// <summary>
    /// Enum to represent different types of messages.
    /// </summary>
    public enum MessageType
    {
        Text,
        Image,
        File,
        Notification,
        KeyExchange,
        ReadReceipt
    }
}
