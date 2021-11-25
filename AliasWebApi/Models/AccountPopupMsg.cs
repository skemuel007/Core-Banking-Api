using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AliasWebApiCore.Models
{
    public class AccountPopupMsg:BaseModel
    {
        public int AccountPopupMsgId { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int BranchId { get; set; }
        [Required(ErrorMessage = "Account Id Required")]
        public int AccountId { get; set; }
        public BranchDetails BranchDetails { get; set; }
        public Account Account { get; set; }
    }
}
