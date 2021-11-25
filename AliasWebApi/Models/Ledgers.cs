using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Ledgers : BaseModel
    {
        [Key]
        public int LedgerId { get; set; }
        public string Name { get; set; }
    }
}