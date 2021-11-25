using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliasWebApiCore.Models
{
    public class SmsConfig:BaseModel
    {
        public int SmsConfigId { get; set; }
        public string HostUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Sender { get; set; }
        public string OptKey { get; set; }
        public string Status { get; set; }
    }
}
