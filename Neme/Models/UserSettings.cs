using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neme.Models
{
    public class UserSettings
    {
        public string DisplayName { get; set; }
        public string Status { get; set; }
        public string AvatarImage { get; set; }
        public bool IsLightTheme { get; set; }
        public bool EnableNotifications { get; set; }
        public bool PlaySound { get; set; }
        public string Port { get; set; }
        public string EncryptionKey { get; set; }
    }
}


