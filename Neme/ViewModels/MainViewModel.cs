using Neme.Models;
using Neme.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Neme.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Neme.Services;

namespace Neme.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ChatMessageService _chatMessageService = new ChatMessageService();

      
        ChatViewModel chatViewModel;
        public MainViewModel()
        {
            chatViewModel = new ChatViewModel();
           
        }
            

       
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
