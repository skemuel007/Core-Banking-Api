using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Group : BaseModel
    {
        [Key]
        public int GroupId { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }
        public virtual ICollection<UserGroup>  UserGroups { get; set; }
    }
}