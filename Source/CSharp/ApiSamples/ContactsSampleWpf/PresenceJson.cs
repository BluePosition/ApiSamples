using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePosition.Samples.ContactsSample
{

    public class Presence
    {
        public string ContactId { get; set; }
        public string BusyState { get; set; }
        public string Details { get; set; }
        public string Source { get; set; }
        public DateTime? ExpectedIdleTime { get; set; }
    }

}
