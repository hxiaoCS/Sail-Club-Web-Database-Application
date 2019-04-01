using System;
using System.Collections.Generic;

namespace HXSail.Models
{
    public partial class Tasks
    {
        public Tasks()
        {
            MemberTask = new HashSet<MemberTask>();
        }

        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<MemberTask> MemberTask { get; set; }
    }
}
