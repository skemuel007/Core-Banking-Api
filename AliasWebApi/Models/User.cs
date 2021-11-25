using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;
using AliasWebApiCore.Models.Identity;

namespace AliasWebApiCore.Models
{
    public class User 
    {

          [Key]
          public int UserId { get; set; }
          [Required]
          [StringLength(150)]
          public string FirstName { get; set; }
          [Required]
          [StringLength(150)]
          public string LastName { get; set; }
          [StringLength(150)]
         public string OtherNames { get; set; }
         public bool? Enabled { get; set; }
         public string Email { get; set; }
         public DateTime DateOfBirth { get; set; }
         [Required]
         public string Type { get; set; }
         [Required]
        public string Username { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public bool Status { get; set; }
        public int? Reset { get; set; }
        public string PasswordChangeFreq { get; set; }
        [StringLength(50)]
        public string ChatName { get; set; }
        [StringLength(100)]
        public string ImageFile { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        //public int? UserGroupId { get; set; }
        public int? BranchId { get; set; }
            public virtual ICollection<Target> Targets { get; set; }
            public virtual ICollection<Sector> Sectors { get; set; }
            public virtual ICollection<Rating> Ratings { get; set; }
            public virtual ICollection<UserGroup> UserGroups { get; set; }
            public virtual ICollection<Individual> Individuals { get; set; }
            public virtual ICollection<Corporate> Corporates { get; set; }
            public virtual ICollection<LoanServicing> LoanServicesCollection { get; set; }
            public virtual ICollection<CountryList> CountryLists { get; set; }
            public virtual IEnumerable<Account> Accounts { get; set; }
            public virtual IEnumerable<BranchDetails> BranchDetails { get; set; }
            public virtual IEnumerable<AccountTypes> AccountTypes { get; set; }
            public virtual IEnumerable<BankDetails> BankDetails { get; set; }
            public virtual IEnumerable<CompanyDirectors> CompanyDirectors { get; set; }
            public virtual IEnumerable<CompanySignatory> CompanySignatories { get; set; }
            public virtual IEnumerable<JointCustomer> JointCustomers { get; set; }
            public virtual IEnumerable<JointCustomersKeys> JointCustomersKeys { get; set; }
            public virtual IEnumerable<Ledgers> Ledgers { get; set; }
            public virtual IEnumerable<Teller> Tellers { get; set; }
            public virtual IEnumerable<GeneralLedgerCode> GeneralLegderCodes { get; set; }
    }
}