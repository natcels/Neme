using System;
using System.Collections.Generic;
using System.Linq;
using Neme.Models;

namespace Neme.Services
{
    public class CallService
    {
        private readonly List<CallSession> _callHistory; // Stores past calls
        private CallSession _activeCall; // Stores the current active call

        public CallService()
        {
            _callHistory = new List<CallSession>();
        }

        // Start a new call
        public CallSession StartCall(List<string> participants, CallType type)
        {
            if (_activeCall != null && _activeCall.Status == CallStatus.Ongoing)
                throw new InvalidOperationException("A call is already in progress.");

            _activeCall = new CallSession
            {
                Participants = participants,
                Type = type,
                Status = CallStatus.Ongoing
            };

            return _activeCall;
        }

        // End the current call
        public void EndCall()
        {
            if (_activeCall == null || _activeCall.Status != CallStatus.Ongoing)
                return;

            _activeCall.EndCall();
            _callHistory.Add(_activeCall);
            _activeCall = null;
        }

        // Mark a missed call
        public void MissedCall(List<string> participants, CallType type)
        {
            var missedCall = new CallSession
            {
                Participants = participants,
                Type = type,
                Status = CallStatus.Missed,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };

            _callHistory.Add(missedCall);
        }

        // Get the call history
        public List<CallSession> GetCallHistory() => _callHistory;

        // Get the active call
        public CallSession GetActiveCall() => _activeCall;
    }
}
