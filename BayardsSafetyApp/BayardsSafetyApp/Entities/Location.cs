using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BayardsSafetyApp.Entities
{
    public class Location
    {
        public string Id_l { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
        public string Content { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
