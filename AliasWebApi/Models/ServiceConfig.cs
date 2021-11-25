using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AliasWebApiCore.Models
{
    public class ServiceConfig:BaseModel
    {
        public int ServiceConfigId { get; set; }
        [Required(ErrorMessage = "Branch Id Required")]
        public int? BranchId { get; set; }
        public string ServiceType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Frequency { get; set; }
        public bool? Status { get; set; }
        public string StartUpType { get; set; }
        public string CronSchedule { get; set; }
        public BranchDetails BranchDetails { get; set; }
    }
}
