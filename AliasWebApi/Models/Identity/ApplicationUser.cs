using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AliasWebApiCore.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool isLoggedIn { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LastLogOut { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
