using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Sector : BaseModel
    {
        [Key]
        public int SectorId { get; set; }

        [Required]
        public string Type { get; set; }
        public string Status { get; set; }
    }
}