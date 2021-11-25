using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class CountryList : BaseModel
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get;  set; }

        public string Desc { get; set; }
        //public int? UserId { get; set; }
        public virtual ICollection<Individual> Individuals { get; set; }
        public virtual ICollection<Corporate> Corporates { get; set; }
        public virtual ICollection<LoanServicing> LoanServicings { get; set; }

    }
}