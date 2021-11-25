using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AliasWebApiCore.Models
{
    public class Loan : BaseModel
    {
        public int LoanId { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}