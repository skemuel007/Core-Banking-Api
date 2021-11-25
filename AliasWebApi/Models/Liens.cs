using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Liens : BaseModel
    {
        [Key]
        public int LienId { get; set; }
        public string LienNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public bool? AutoRelease { get; set; }
        public string Status { get; set; }
        public int? ApprovedUserId { get; set; }
        public string ApprovedNote { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? RejectedUserId { get; set; }
        public string RejectedNote { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        public BranchDetails BranchDetails { get; set; }
        [Required(ErrorMessage = "Account Id Required")]
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}