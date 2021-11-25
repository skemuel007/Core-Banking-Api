using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class CompanyDirectors : BaseModel
    {
        [Key]
        public int DirectorId { get; set; }

        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string Contacts { get; set; }
        public string IdType { get; set; }
        public string IdNumber { get; set; }
        [Required(ErrorMessage = "Corporate Id Required")]
        public int CorporateCustId { get; set; }
        public Corporate Corporate { get; set; }
    }
}