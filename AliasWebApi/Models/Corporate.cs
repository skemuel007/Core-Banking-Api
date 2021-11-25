using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Corporate : BaseModel
    {
        [Key]
        public int CorporateCustId { get; set; }
        [Required]
        public string CorporateNumber { get; set; }
        public string CompanyName { get; set; }
        public string NatureOfBusiness { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime DateOfIncorporation { get; set; }
        public string ComapnyLocation { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyWebsite { get; set; }
        public string TinNumber { get; set; }
        public int? RelationalOfficer { get; set; }
        public bool Placement { get; set; }
        public string Address { get; set; }
        public bool BroadcastAlert { get; set; }
        public bool SupportAlert { get; set; }
        //[Required(ErrorMessage = "CustomerType Id Required")]
        //public int CustomerTypeId { get; set; }
        //public CustomerType CustomerType { get; set; }

        public virtual ICollection<CompanySignatory> CompanySignatory { get; set; }
        public virtual ICollection<CompanyDirectors> CompanyDirectors { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<RelatedDocument> RelatedDocuments { get; set; }
    }
}