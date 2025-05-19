using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neme.Models
{
    public class SharedFile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; }
        public string FilePath { get; set; } // local path
        public string OwnerUsername { get; set; }
        public List<string> Collaborators { get; set; } = new();
        public DateTime UploadedAt { get; set; } = DateTime.Now;

    }
}
