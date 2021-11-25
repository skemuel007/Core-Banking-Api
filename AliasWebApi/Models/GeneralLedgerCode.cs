using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class GeneralLedgerCode : BaseModel
    {
        [Key]
        public int  GeneralLedgerCodeId { get; set; }

        public string GLType { get; set; }
        public string SubCode { get; set; }
        public string Description { get; set; }
        public string BalanceType { get; set; }
        public bool AllowJournals { get; set; }
        public bool Status { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Banktiers> Banktiers { get; set; }
        [Required(ErrorMessage = "Main Gl Id Required")]
        public int MainGeneralLedgerCodeId { get; set; }
        public MainGeneralLedgerCodes MainGeneralLedgerCodes { get; set; }
    }
}