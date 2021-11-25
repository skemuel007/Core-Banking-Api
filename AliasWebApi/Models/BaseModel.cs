using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AliasWebApiCore.Models
{
    public abstract  class BaseModel
    {
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        //[ForeignKey("CreatedUserId")]
        //public virtual User User { get; set; }
    }
}