using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class JointCustomer : BaseModel
    {
        [Key]
        public int JointId { get; set; }
        [Required]
        public string JointNumber { get; set; }
        public bool GeneralBroadAlert { get; set; }
        public bool SupportAlert { get; set; }
        public virtual ICollection<JointCustomersKeys> JointCustomersKeys { get; set; }
        public  ICollection<Account> Accounts { get; set; }
    }
}