using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class CashierSummary
    {
        public string CashierName { get; set; }
        public string UserName { get; set; }
        public string BranchName { get; set; }
        public int Transactions { get; set; }
        public  decimal Balance { get; set; }
        public string Pin { get; set; }
        public decimal WithDrawalLimit { get; set; }
        [Required(ErrorMessage = "Teller Id Required")]
        public int TellerId { get; set; }
        public decimal DepositLimit { get; set; }
        public Teller Teller { get; set; }
    }
}