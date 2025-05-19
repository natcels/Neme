using Neme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neme.helpers
{
    public static class FileAccessManager
    {
        public static bool CanAccessFile(SharedFile file, string requestingUser)
        {
            if (file.OwnerUsername == requestingUser)
                return true;

            return file.Collaborators.Contains(requestingUser)
                   && SessionManager.IsUserOnline(file.OwnerUsername);
        }
    }
}
