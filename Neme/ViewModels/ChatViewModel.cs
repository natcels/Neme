using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Neme.Models;

namespace Neme.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public ObservableCollection<ChatMessage> Messages { get; set; }
        public string NewMessage { get; set; }
        public ICommand SendMessageCommand { get; }

        public ChatViewModel()
        {
            Messages = new ObservableCollection<ChatMessage>();
            SendMessageCommand = new RelayCommand(SendMessage);

            // Sample Messages
            Messages.Add(new ChatMessage { Content = "Hey there!", IsSentByCurrentUser = false, SenderName = "Alice", Timestamp = DateTime.Now });
            Messages.Add(new ChatMessage { Content = "Hello! How are you?", IsSentByCurrentUser = true, SenderName = "You", Timestamp = DateTime.Now });
        }

        private void SendMessage(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                Messages.Add(new ChatMessage
                {
                    Content = NewMessage,
                    IsSentByCurrentUser = true,
                    SenderName = "You",
                    Timestamp = DateTime.Now
                });

                // Clear input after sending
                NewMessage = string.Empty;
                OnPropertyChanged(nameof(NewMessage));
            }
        }
    }
}


