using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class CustomerType : BaseModel
    {
        public int CustomerTypeId { get; set; }
        [Required]
        public string Type { get; set; }
    }
}