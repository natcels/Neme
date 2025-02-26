using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neme.Models
{
    public class CallSignal
    {
        public string Type { get; set; }   // call_request, call_accepted, call_rejected, call_ended
        public string Sender { get; set; } // Who is making the call?
        public string Recipient { get; set; } // Who is receiving the call?
        public string CallType { get; set; } // Voice or Video
    }
}
