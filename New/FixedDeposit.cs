using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class FixedDeposit
    {
    
        public int FixedDepositId { get; set; }
        public string AccountNumber { get; set; }
        public string CustomerNumber { get; set; }
        
	
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
        public int? FixedDepositFundingSourceAccountId { get; set; }
        public DateTime? NextFixedDepositInterestAutoApplyDate { get; set; }
        public DateTime? FixedDepositInterestAutoApplyEndDate { get; set; }
		public int? AutoApplyFixedDepositInterest { get; set; }
        public int? DaysToAutoApplyFixedDepositInterest { get; set; }
        public bool? RolloverInterest { get; set; }
        public bool? RolloverPrincipal { get; set; }
		public bool? InvestmentAlert { get; set; }
		
		//public bool Placement { get; set; }
		
		public DateTime? AccountClosureDate { get; set; }
        public DateTime? OpenedDate { get; set; }
		public string AccountStatus { get; set; }
        public int? ApprovedUserId { get; set; }
        public DateTime? ApprovedDateTime { get; set; }
        public int? RejectedUserId { get; set; }
        public int? RelationsOfficerId { get; set; }
        public DateTime? InvIntLastAccruedDate { get; set; }
		//individual customer details
        public int? IndividualCustId { get; set; }
        public virtual Individual Individual { get; set; }

        //Corporate customer details
        public int? CorporateCustId { get; set; }
        public virtual Corporate Corporate { get; set; }

        //Joint customer
        public int? JointId { get; set; }
        public JointCustomer JointCustomer { get; set; }

        //branch details
        public int? BranchId { get; set; }
        public BranchDetails BranchDetails { get; set; }
		
		//Account Type
        public int? AccountTypeId { get; set; }
        public AccountTypes AccountTypes { get; set; }
        //Creation information
        public string ClosureReason { get; set; }
        public int? ClosedUserId { get; set; }
        public int? CreatedUserId { get; set; }
        public User User { get; set; }

        public DateTime? CreatedDate { get; set; }

      
	}
}