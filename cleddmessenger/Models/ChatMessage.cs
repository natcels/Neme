using System.ComponentModel;

namespace Neme.Models
{

    public class ChatMessage : INotifyPropertyChanged
    {


        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsSentByCurrentUser { get; set; }
        public DateTime Timestamp { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string AvatarImage { get; set; }
        public MessageStatus DeliveryStatus { get; set; }
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
        Notification
    }
}