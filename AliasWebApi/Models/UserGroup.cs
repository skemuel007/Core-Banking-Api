using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class UserGroup : BaseModel
    {
        public virtual Group Group { get; set; }
        public virtual  int GroupId { get; set; }
    }
}