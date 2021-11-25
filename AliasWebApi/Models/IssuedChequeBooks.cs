using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class IssuedChequeBooks : BaseModel
    {
        [Key]
        public int IssuedChequeBookId { get; set; }
        public int StartNumber { get; set; }
        public int EndNumber { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        public BranchDetails BranchDetails { get; set; }
        [Required(ErrorMessage = "Account Id Required")]
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}