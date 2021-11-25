using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class BranchDetails:BaseModel
    {
        [Key]
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public string BranchName { get; set; }
        [Required]
        public string BranchCode { get; set; }
        public string PostalAddress { get; set; }
        public string LocationAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public string Databasefile { get; set; }
        public string Region { get; set; }
        public string District { get; set; }
        public string Area { get; set; }
        public string AccountFile { get; set; }
        public string Status { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
       

    }
}