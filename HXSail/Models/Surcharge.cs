using System;
using System.Collections.Generic;

namespace HXSail.Models
{
    public partial class Surcharge
    {
        public int SurchargeId { get; set; }
        public int? StartYear { get; set; }
        public int? EndYear { get; set; }
        public double? Amount { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
