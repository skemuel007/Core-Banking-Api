using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class ResponseFormat
    {
        public string Status { get; set; }
        public decimal? TotalBalance { get; set; }
        public decimal? AvailableBalance { get; set; }

    }
}