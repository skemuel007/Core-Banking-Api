using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class TransCodeItems : BaseModel
    {
        public int TransCodeItemsId { get; set; }
        [StringLength(10)]
        public string LedgerType { get; set; }
        public int? LoanId { get; set; }
        [StringLength(50)]
        public string TransCode { get; set; }
        [StringLength(100)]
        public string Reference { get; set; }
        [Column(TypeName = "Money")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "Money")]
        public decimal? Credit { get; set; }
        [StringLength(50)]
        public string TransSource { get; set; }
        public string Status { get; set; }
        public DateTime? SessionDate { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        public BranchDetails BranchDetails { get; set; }
        public int? AccountId { get; set; }
        public Account Account { get; set; }
        public int? GeneralLedgerCodeId { get; set; }
        public GeneralLedgerCode GeneralLedgerCode { get; set; }
    }
}