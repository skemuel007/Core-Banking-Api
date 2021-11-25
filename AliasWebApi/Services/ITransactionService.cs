using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;

namespace AliasWebApiCore.Services
{
    public interface ITransactionService
    {
        string GetTransCode(string type);
        string GetAccountNumber(string branchCode, string accountTypeCode, string customerNumber);
        void LogSmsJoint(Account account,string transcode, string operation, decimal amount, Transaction transaction = null, string accounttype = null);
        void LogSmsCorporate(Account account,string transcode, string operation,decimal amount,Transaction transaction= null, string accounttype = null);
        void LogSmsIndividual(Account account,string transcode, string operation,decimal amount, Transaction transaction = null,string accounttype=null);
        void SendForApproval(string status, TransactionAccept transaction,string transcode);

        void ConfigureAccountTransaction(string type, Account account,  Transaction transaction = null, string transactioncode = null, TransCodeItems transcodeitems = null/*, string chequeNumber = null*/);
    }
}
