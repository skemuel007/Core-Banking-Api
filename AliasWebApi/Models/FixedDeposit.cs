using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AliasWebApiCore.Models
{
    public class FixedDeposit 
    {
        public int FixedDepositId { get; set; }
        public int? FixedDepositServicingAccountId { get; set; }
        public int? FixedDepositPeriod { get; set; }
        public decimal? FixedDepositInterestRate { get; set; }
        public DateTime? FixedDepositMaturityDate { get; set; }
        [Column(TypeName = "Money")]
        public decimal? FixedDepositMaturityAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal? FixedDepositPrincipal { get; set; }
        [Column(TypeName = "Money")]
        public decimal? FixedDepositInterestAmount { get; set; }
        public decimal? NewInterestRate { get; set; }
        public int? NewPeriod { get; set; }
        [Column(TypeName = "Money")]
        public decimal? FixedDepositDailyInterest { get; set; }
        [Column(TypeName = "Money")]
        public decimal? FixedDepositInterestAccrued { get; set; }
        [Required(ErrorMessage = "Funding source id required")]
        public int FixedDepositFundingSourceAccountId { get; set; }
        public DateTime? NextFixedDepositInterestAutoApplyDate { get; set; }
        public DateTime? FixedDepositInterestAutoApplyEndDate { get; set; }
        public int? AutoApplyFixedDepositInterest { get; set; }
        public int? DaysToAutoApplyFixedDepositInterest { get; set; }
        public bool? RolloverInterest { get; set; }
        public bool? RolloverPrincipal { get; set; }
        public bool? InvestmentAlert { get; set; }
        public DateTime? InvIntLastAccruedDate { get; set; }
        public int AccountId { get; set; }
       
        public Account Account { get; set; }
    }

}
