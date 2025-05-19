using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neme.Models
{
    public class Reminder
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
