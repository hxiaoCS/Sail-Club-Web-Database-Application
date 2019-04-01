using System;
using System.Collections.Generic;

namespace HXSail.Models
{
    public partial class AnnualFeeStructure
    {
        public int Year { get; set; }
        public double? AnnualFee { get; set; }
        public double? EarlyDiscountedFee { get; set; }
        public DateTime? EarlyDiscountEndDate { get; set; }
        public DateTime? RenewDeadlineDate { get; set; }
        public double? TaskExemptionFee { get; set; }
        public double? SecondBoatFee { get; set; }
        public double? ThirdBoatFee { get; set; }
        public double? ForthAndSubsequentBoatFee { get; set; }
        public double? NonSailFee { get; set; }
        public DateTime? NewMember25DiscountDate { get; set; }
        public DateTime? NewMember50DiscountDate { get; set; }
        public DateTime? NewMember75DiscountDate { get; set; }

        public virtual Membership Membership { get; set; }
    }
}
