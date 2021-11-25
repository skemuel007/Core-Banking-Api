using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Transaction : BaseModel
    {
        [Key]
        public int TransactionId { get; set; }
        [Required(ErrorMessage = "Ledger Id Required")]
        public int LedgerId { get; set; }
        public Ledgers Ledgers { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        public BranchDetails BranchDetails { get; set; }

        public int? LoanId { get; set; }
        public Loan Loan { get; set; }
        //whether the sms has been sent or not
        public bool? SmsStatus { get; set; }
        public int? GeneralLedgerCodeId { get; set; }

        public int? AccountId { get; set; }
        public Account Account { get; set; }

        public string TransSource { get; set; }
        public string LedgerType { get; set; }
        public DateTime? SessionDate { get; set; }
        [Column(TypeName = "Money")]
        public decimal Debit { get; set; }
        [Column(TypeName = "Money")]
        public decimal Credit { get; set; }
        [Column(TypeName = "Money")]
        public decimal? Balance { get; set; }
        public string Reference { get; set; }
        public string TransCode { get; set; }
        public string MacAddress { get; set; }
        public string ChequeNumber { get; set; }
        public bool? ReconciledState { get; set; }
        public int? ReconciledUserId { get; set; }
        public DateTime? ReconciledSessionDate { get; set; }
        public GeneralLedgerCode GeneralLedgerCode { get; set; }


    }
}