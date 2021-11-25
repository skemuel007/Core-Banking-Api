using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class BankDetails : BaseModel
    {
        public int BankDetailsId { get; set; }
        public string CompanyName { get; set; }
        public string PostalAddress { get; set; }
        public string LocationAddress { get; set; }
        public string Logo { get; set; }
        public string ServerMacId { get; set; }
        public string DatabaseId { get; set; }
        public string ActFile { get; set; }
    }
}