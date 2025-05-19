using Neme.Data;
using Neme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neme.Services
{
    public class ReminderService
    {
        public static void AddReminder(Reminder reminder)
        {
            DatabaseHelper.AddReminder(reminder);
        }

        public static IEnumerable<Reminder> GetAllReminders()
        {
           return DatabaseHelper.GetAllReminders();
        }
    }
}
