using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Neme.Models;
using Neme.Services;

namespace Neme.ViewModels
{
    public class CallHistoryViewModel : BaseViewModel
    {
        private readonly VoiceCallService _callService;

        public ObservableCollection<CallSession> CallHistory { get; }

        private CallSession _selectedCall;
        public CallSession SelectedCall
        {
            get => _selectedCall;
            set
            {
                _selectedCall = value;
                OnPropertyChanged();
            }
        }

        public CallHistoryViewModel(VoiceCallService callService)
        {
            _callService = callService;
      //      CallHistory = new ObservableCollection<CallSession>(_callService.GetCallHistory());
        }

        public void RefreshCallHistory()
        {
            CallHistory.Clear();
            //foreach (var call in _callService.GetCallHistory().OrderByDescending(c => c.StartTime))
            //{
            //    CallHistory.Add(call);
            //}
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
