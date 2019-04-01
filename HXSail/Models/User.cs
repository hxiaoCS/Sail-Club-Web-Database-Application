using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HXSail.Models
{
    public class User : IdentityUser
    {
        public string voterId { get; set; }
    }
}
