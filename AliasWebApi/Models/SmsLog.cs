using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AliasWebApiCore.Models
{
    public class SmsLog
    {
        public int SmsLogId { get; set; }
        public string TransCode { get; set; }
        public string Message { get; set; }
        public int? ResponseCode { get; set; }
        public bool? Status { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int? BranchId { get; set; }
        [Required(ErrorMessage = "Account Id Required")]
        public int? AccountId { get; set; }
        public virtual BranchDetails BranchDetails { get; set; }
        public virtual Account Account { get; set; }
    }
}
