using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluePosition.Samples.ContactsSample
{

    public class Contact
    {
        public string Id { get; set; }
        public string ScaleUserId { get; set; }
        public Availablefeature[] AvailableFeatures { get; set; }
    }

    public class Availablefeature
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }

}
