using Neme.Models;
using System.Collections.Generic;

namespace Neme.State
{
    public class AppState
    {
        private static AppState instance;
        public static AppState Instance => instance ??= new AppState();
        private AppState() { }

        public User CurrentUser { get; set; }

        public List<Peer> DiscoveredPeers { get; set; } = new();

        public List<AppTask> Tasks { get; set; } = new();

        public List<Reminder> Reminders { get; set; } = new();

    }
}
