using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Cheques : BaseModel
    {
        [Key]
        public int ChequeId { get; set; }
        [Column(TypeName = "Money")]
        public decimal ChequeAmount { get; set; }
        public string ChequeNumber { get; set; }
        public string Status { get; set; }
        public bool? AutoClear { get; set; }
        public DateTime? ClearedDate { get; set; }
        public int? ClearedUserId { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public int? ReturnedUserId { get; set; }
        public string Note { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        public BranchDetails BranchDetails { get; set; }
        [Required(ErrorMessage = "Account Id Required")]
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}