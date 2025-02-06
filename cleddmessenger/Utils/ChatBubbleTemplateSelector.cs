using System;
using System.Windows;
using System.Windows.Controls;
using Neme.Models;

namespace Neme.Utils
{
    public class ChatBubbleTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SentMessageTemplate { get; set; }
        public DataTemplate ReceivedMessageTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var message = item as ChatMessage; // Assuming ChatMessage is your model
            if (message != null)
            {
                return message.IsSentByCurrentUser ? SentMessageTemplate : ReceivedMessageTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }

}


