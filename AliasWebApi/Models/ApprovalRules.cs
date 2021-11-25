using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class ApprovalRules : BaseModel
    {
        [Key]
        public int ApprovalRulesId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public string ApproversUserIds { get; set; }
        [Column(TypeName = "Money")]
        public decimal MinimumAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal MaximumAmount { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        public BranchDetails BranchDetails { get; set; }
    }
}