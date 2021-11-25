using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class CommonSequence : BaseModel
    {
        public int CommonSequenceId { get; set; }
        public string Name { get; set; }
        public int Counter { get; set; }
        public int FixedLength { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
    }
}