using System;
using System.Collections.Generic;

namespace HXSail.Models
{
    public partial class Member
    {
        public Member()
        {
            Boat = new HashSet<Boat>();
            MemberTask = new HashSet<MemberTask>();
            Membership = new HashSet<Membership>();
        }

        public int MemberId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SpouseFirstName { get; set; }
        public string SpouseLastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string Email { get; set; }
        public int? YearJoined { get; set; }
        public string Comment { get; set; }
        public bool TaskExempt { get; set; }
        public bool UseCanadaPost { get; set; }

        public Province ProvinceCodeNavigation { get; set; }
        public ICollection<Boat> Boat { get; set; }
        public ICollection<MemberTask> MemberTask { get; set; }
        public ICollection<Membership> Membership { get; set; }
    }
}
