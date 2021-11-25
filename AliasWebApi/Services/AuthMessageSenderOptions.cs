using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliasWebApiCore.Services
{
    public class AuthMessageSenderOptions
    {
        public string FromEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
