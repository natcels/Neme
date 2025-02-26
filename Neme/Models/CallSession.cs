using System;
using System.Collections.Generic;


namespace Neme.Models
{
    public class CallSession
    {
        public Guid CallId { get; set; }  // Unique identifier for each call
        public List<string> Participants { get; set; } // List of users in the call
        public DateTime StartTime { get; set; }  // When the call started
        public DateTime? EndTime { get; set; }   // When the call ended (nullable for ongoing calls)
        public CallType Type { get; set; }       // Voice or Video call
        public CallStatus Status { get; set; }   // Ongoing, Completed, Missed
        public int? CallDuration => EndTime.HasValue ? (int)(EndTime.Value - StartTime).TotalSeconds : null;
        public double? CallQuality { get; set; } // Optional: Network quality rating (1-5)

        public CallSession()
        {
            CallId = Guid.NewGuid();
            Participants = new List<string>();
            StartTime = DateTime.Now;
            Status = CallStatus.Ongoing;
        }

        public void EndCall()
        {
            EndTime = DateTime.Now;
            Status = CallStatus.Completed;
        }
    }

    public enum CallType
    {
        Voice,
        Video
    }

    public enum CallStatus
    {
        Ongoing,
        Completed,
        Missed
    }
}

