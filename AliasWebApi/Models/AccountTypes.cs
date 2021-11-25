using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class AccountTypes : BaseModel
    {

        [Key]
        public int AccountTypeId { get; set; }
        public string AccountType { get; set; }
        [Required]
        public string AccountTypeCode { get; set; }
        public int ApprovalRuleId { get; set; }
        public int GeneralLedgerCodeId { get; set; }
        [Column(TypeName = "Money")]
        public decimal MinimumBalanceBeforeInterestIsPayable { get; set; }
        public string BaseType { get; set; }
        public bool AllowOverdrawnBalance { get; set; }
        public int SequenceFormatId { get; set; }
        public int NumberOfDaystoClassifyAccountAsDormant { get; set; }
        public int? AccountAgeBeforeInterestIsPayable { get; set; }
        public bool AllowInterestOnDormantAccount { get; set; }

        public string SavingsInterestCalculationMethod { get; set; }
        public int? SavingsInterestTypeId { get; set; }
        [ForeignKey("SavingsInterestTypeId")]
        public Banktiers Banktiers { get; set; }
        public int COTTypeID { get; set; }
        public bool AutoApplyCOT { get; set; }
        public string AutoCOTApplicationFreq { get; set; }

        public string AutoApplySavingsInterestFreq { get; set; }
        public bool AutoApplySavingsInterest { get; set; }
        public int? GlCodeForFDPenalty { get; set; }
        public int? GLCodeForFDInterestAccrued { get; set; }
        public int? GLCodeForFDInterestExpense { get; set; }
        public string AllowFDBackDating { get; set; }
        public int? NumberOfDaysInYearForFDInterestCal { get; set; }
        public bool InvestmentPlacement { get; set; }

        [Column(TypeName = "Money")]
        public decimal MinimumInitialDepositAmount { get; set; }
        public int WaitingPeriodBeforeFirstWithdrawal { get; set; }
        public int WithdrawalFrequency { get; set; }
        public int? MinimumFixedDepositTenor { get; set; }
        public int? MaximumFixedDepositTenor { get; set; }
        [Required(ErrorMessage = "Ledger Id Required")]
        public int LedgerId { get; set; }
        public string LedgerType { get; set; }
        public virtual GeneralLedgerCode GeneralLedgerCode { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual Ledgers Ledgers { get; set; }
    }
}