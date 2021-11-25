using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class RelatedDocument : BaseModel
    {
        [Key]
        public int RelatedDocumentId { get; set; }
        [Required(ErrorMessage = "Corporate Id Required")]
        public int CorporateCustId { get; set; }
        public Corporate Corporate { get; set; }
    }
}