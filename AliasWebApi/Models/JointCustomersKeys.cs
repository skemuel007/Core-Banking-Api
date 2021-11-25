using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class JointCustomersKeys : BaseModel
    {
        [Key]
        public int JointCustKeysId { get; set; }
        [Required(ErrorMessage = "Individual Id Required")]
        public int IndividualCustId { get; set; }
        [ForeignKey("IndividualCustId")]
        public Individual Individual { get; set; }
        [Required(ErrorMessage = "Joint Id Required")]
        public int JointId { get; set; }
        [ForeignKey("JointId")]
        public virtual JointCustomer JointCustomer { get; set; }
    }
}