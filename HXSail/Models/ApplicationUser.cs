using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HXSail.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
        public bool IsMember { get; set; }
        public bool IsLocal { get; set; }
        public bool IsLocked { get; set; }
    }
}

