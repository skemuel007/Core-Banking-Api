using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class MainGeneralLedgerCodes : BaseModel
    {
        [Key]
        public int MainGeneralLedgerCodeId { get; set; }
        [StringLength(20)]
        public string Code { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        public bool? Status { get; set; }
    }
}