using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Banktiers : BaseModel
    {
        public int BanktiersId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool FlatRate { get; set; }
        [Column(TypeName = "Money")]
        public decimal FlatRateAmount { get; set; }
        public bool Percentage { get; set; }
        //public string GLCode { get; set; }
        [Column(TypeName = "Money")]
        public decimal MinimumAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal MaximumAmount { get; set; }
        public decimal ChargeXAmountForActivityPeriod { get; set; }
        public bool ConsiderOverdraftForPeriod { get; set; }
        public DateTime ChargeXAmountForActivityPeriodDate { get; set; }
        public double PercentageValue { get; set; }
        //public virtual ICollection<Account> Accounts { get; set; }
        [Required(ErrorMessage = "GeneralLedgerCode Id Required")]
        public int GeneralLedgerCodeId { get; set; }
        public virtual GeneralLedgerCode GeneralLedgerCode { get; set; }
    }
}