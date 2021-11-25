using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class DepositWithdrawal
    {
        [Required]
        public string Type { get; set; }
        [Required]
        public string Operation { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public int AccountId { get; set; }
    }
}