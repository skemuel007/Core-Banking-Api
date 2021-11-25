using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class CompanySignatory : BaseModel
    {
        [Key]
        public int SignatoryId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string Contact { get; set; }
        public string Type { get; set; }
        public string Picture { get; set; }
        public string Signature { get; set; }
        [Required(ErrorMessage = "Corporate Id Required")]
        public int CorporateCustId { get; set; }
        public Corporate Corporate { get; set; }
    }
}