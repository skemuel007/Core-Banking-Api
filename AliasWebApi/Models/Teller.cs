using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Teller : BaseModel
    {
        [Key]
        public int TellerID { get; set; }
        public string Pin { get; set; }
        [Column(TypeName = "Money")]
        //goes for approval once the withdrawal limit has been exceeded
        public decimal WithdrawalLimit { get; set; }
        [Column(TypeName="Money")]
        public decimal DepositLimit { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public bool? LoginStatus { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        [Required(ErrorMessage = "GeneralLedgerCode Id Required")]
        public int GeneralLedgerCodeId { get; set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        public virtual BranchDetails BranchDetails{get;set;}
        public virtual GeneralLedgerCode GeneralLedgerCode { get; set; }
    }
}