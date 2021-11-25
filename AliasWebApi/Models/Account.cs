using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Account:BaseModel
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerNumber { get; set; }
        public int? InterestOnSavingsId { get; set; }
        public bool? SalaryAccount { get; set; }
        public bool? TransactionAlert { get; set; }
        public bool? InternetBanking { get; set; }
        public bool? MobileBanking { get; set; }
        public bool? MobileMoney { get; set; }
        public bool? Atm { get; set; }
        public bool? LoanPaymentAlert { get; set; }
        public string PurposeId { get; set; }
        public DateTime? AccountClosureDate { get; set; }
        public DateTime? OpenedDate { get; set; }
        [Column(TypeName = "Money")]
        public decimal TotalBalance { get; set; }
        [Column(TypeName = "Money")]
        public decimal AvailableBalance { get; set; }
        public string AccountStatus { get; set; }
        public int? ApprovedUserId { get; set; }
        public DateTime? ApprovedDateTime { get; set; }
        public int? RejectedUserId { get; set; }
        public int? RelationsOfficerId { get; set; }
        public string ClosureReason { get; set; }
        public int? ClosedUserId { get; set; }
        public bool SyncStatus { get; set; }
        public int? IndividualCustId { get; set; }
        public virtual Individual Individual { get; set; }
        public int? CorporateCustId { get; set; }
        public virtual Corporate Corporate { get; set; }
        public int? JointId { get; set; }
        public JointCustomer JointCustomer { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        public BranchDetails BranchDetails { get; set; }
        [Required(ErrorMessage = "Account Type Required")]
        public int AccountTypeId { get; set; }
        public AccountTypes AccountTypes { get; set; }
    }
}