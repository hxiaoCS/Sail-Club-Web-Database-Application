using System;
using System.Collections.Generic;

namespace HXSail.Models
{
    public partial class BoatType
    {
        public BoatType()
        {
            Boat = new HashSet<Boat>();
            Parking = new HashSet<Parking>();
        }

        public int BoatTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Chargeable { get; set; }
        public bool Sail { get; set; }
        public string Image { get; set; }

        public ICollection<Boat> Boat { get; set; }
        public ICollection<Parking> Parking { get; set; }
    }
}
