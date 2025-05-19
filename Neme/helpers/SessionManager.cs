using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neme.helpers
{
    public static class SessionManager
    {
        private static readonly HashSet<string> _activeUsers = new();

        public static void UserLoggedIn(string username) => _activeUsers.Add(username);
        public static void UserLoggedOut(string username) => _activeUsers.Remove(username);
        public static bool IsUserOnline(string username) => _activeUsers.Contains(username);
    }
}
