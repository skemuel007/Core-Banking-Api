using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AliasWebApiCore.Models
{
    public class SessionManager:BaseModel
    {
        public int SessionManagerId { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int? BranchId { get; set; }
        public DateTime? SessionDate { get; set; }
        public string Status { get; set; }
        public int? ClosedUserId { get; set; }
        public DateTime? ClosedDate { get; set; }
        [NotMapped]
        public string BranchName { get; set; }
        public BranchDetails BranchDetails { get; set; }
    }
}
