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
        public int ApprovalRuleId { get; set; }
        public int GeneralLedgerCodeId {get; set; }
        [Column(TypeName = "Money")]
        public string BaseType { get; set; }
        public int SequenceFormatId { get; set; }

        public int? GlCodeForFDPenalty { get; set; }
        public int? GLCodeForFDInterestAccrued { get; set; }
        public int? GLCodeForFDInterestExpense { get; set; }
        public string AllowFDBackDating { get; set; }
        public int? NumberOfDaysInYearForFDInterestCal { get; set; }
        public bool InvestmentPlacement { get; set; }
      
        public int? MinimumFixedDepositTenor { get; set; }
        public int? MaximumFixedDepositTenor { get; set; }


        //Creation information
        public virtual User User { get; set; }
        public int? LedgerId { get; set; }
        //Account Navigational
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual Ledgers Ledgers{get;set;}

        

    }
}