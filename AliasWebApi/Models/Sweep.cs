using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Sweep : BaseModel
    {
        [Key]
        public int SweepId { get; set; }
        [Required(ErrorMessage ="The source account id is required")]
        public int FromAccountId { get; set; }
        [Required(ErrorMessage = "The destination account id is required")]
        public int ToAccountId { get; set; }
        [Column(TypeName = "Money")]
        public decimal MinimumBalance { get; set; }
        public DateTime StartDate { get; set; }
        public string Frequency { get; set; }
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
        public DateTime? RejectedDate { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        public BranchDetails BranchDetails { get; set; }
        public string Type { get; set; }
        public int? TransferDay { get; set; }
        [NotMapped]
        public string FromAccountNumber { get; set; }
        [NotMapped]
        public string ToAccountNumber { get; set; }
    }
}