using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Services
{
    public class TransactionService:ITransactionService
    {
        private readonly AppDbContext _db;

        public TransactionService(AppDbContext db)
        {
            _db = db;
        }

        public string GetTransCode(string type)
        {
            CommonSequence sequence = _db.CommonSequences.FirstOrDefault(a => a.Name.ToLower().Contains(type.ToLower()));
            if (sequence == null) return null;
            string transcodenum = (sequence.Counter + 1).ToString();
            sequence.Counter += 1;
            _db.Entry(sequence).State = EntityState.Modified;
            _db.SaveChanges();
            int length = sequence.FixedLength;
            int padnum = length - transcodenum.Length;
            string transnumber = transcodenum.PadLeft(padnum+1, '0');
            string transcode = sequence.Prefix + transnumber + sequence.Suffix;
            return transcode;
        }

        public string GetAccountNumber(string branchCode,string accountTypeCode,string customerNumber)
        {

            CommonSequence sequence = _db.CommonSequences.FirstOrDefault(a => a.Name.ToLower().Contains("customer"));
            string accountNumber = null;
            if (sequence != null)
            {
                string transcodenum = (sequence.Counter + 1).ToString();
                sequence.Counter += 1;
                _db.Entry(sequence).State = EntityState.Modified;
                _db.SaveChanges();
                int length = sequence.FixedLength;
                int padnum = length - transcodenum.Length;
                string transnumber = transcodenum.PadLeft(padnum + 1, '0');
                accountNumber = sequence.Prefix + branchCode + accountTypeCode + customerNumber + transnumber +
                                    sequence.Suffix;
            }
            return accountNumber;
        }



        public void LogSmsJoint(Account account,string transcode, string operation, decimal amount, Transaction transaction = null, string accounttype = null)
        {
            var individuals = _db.JointCustomersKeys.Where(a => a.JointId == account.JointId).Select(b => b.Individual).ToList();
            foreach (var individual in individuals)
            {
                string fullname = individual.FirstName + " " + individual.LastName;
                var smslog = new SmsLog
                {
                    AccountId = account.AccountId,
                    Status = false,
                    BranchId = transaction?.BranchId,
                    Message = accounttype != "fd" ? $"Dear {fullname}, there has been a {operation} transaction on your account {account.AccountNumber.Substring(4, 4)}xxxxxx{account.AccountNumber.Substring(account.AccountNumber.Length - 2)} of GHS{amount} at {DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()} .Your current balance is GHS" :
                        $"Dear {fullname}, there has been a {operation}  on your Fixed Deposit Account {account.AccountNumber.Substring(4, 4)}xxxxxx{account.AccountNumber.Substring(account.AccountNumber.Length - 2)} of GHS{amount} at {DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()} .Your current principal is GHS",
                    TransCode = transcode,
                    CreatedDate = DateTime.Now,
                    PhoneNumber = individual.KPhone
                };
            }
        }

        public void LogSmsCorporate(Account account,string transcode, string operation, decimal amount, Transaction transaction=null, string accounttype = null)
        {
            string phonenumber =
            (_db.Corporates.Where(a => a.CorporateCustId == account.CorporateCustId)
                .Select(b => b.CompanyPhone)).FirstOrDefault();
            var corporate = _db.Corporates.FirstOrDefault(a => a.CorporateCustId == account.CorporateCustId);
            string fullname = corporate != null ? corporate.CompanyName : null;
            var smslog = new SmsLog
            {
                AccountId = account.AccountId,
                Status = false,
                BranchId = transaction?.BranchId,
                Message = accounttype != "fd" ? $"Dear {fullname}, there has been a {operation} transaction on your account {account.AccountNumber.Substring(4, 4)}xxxxxx{account.AccountNumber.Substring(account.AccountNumber.Length - 2)} of GHS{amount} at {DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()} .Your current balance is GHS" :
                    $"Dear {fullname}, there has been a {operation} on your Fixed Deposit Account {account.AccountNumber.Substring(4, 4)}xxxxxx{account.AccountNumber.Substring(account.AccountNumber.Length - 2)} of GHS{amount} at {DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()} .Your current principal is GHS",
                TransCode = transcode,
                CreatedDate = DateTime.Now,
                PhoneNumber = phonenumber
            };
            _db.SmsLog.Add(smslog);
            _db.SaveChanges();
        }

        public void LogSmsIndividual(Account account, string transcode, string operation, decimal amount, Transaction transaction = null, string accounttype=null)
        {
            string phonenumber =
            (_db.Individuals.Where(a => a.IndividualCustId == account.IndividualCustId)
                .Select(b => b.KPhone)).FirstOrDefault();
            var individual = _db.Individuals.FirstOrDefault(a => a.IndividualCustId == account.IndividualCustId);
            string fullname = individual != null ? individual.FirstName + " " + individual.LastName : null;
            var smslog = new SmsLog
            {
                AccountId = account.AccountId,
                Status = false,
                BranchId = transaction?.BranchId,
                Message =accounttype!="fd"? $"Dear {fullname}, there has been a {operation} transaction on your account {account.AccountNumber.Substring(4, 4)}xxxxxx{account.AccountNumber.Substring(account.AccountNumber.Length - 2)} of GHS{amount} at {DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()} .Your current balance is GHS":
                    $"Dear {fullname}, there has been a {operation} on your Fixed Deposit Account {account.AccountNumber.Substring(4, 4)}xxxxxx{account.AccountNumber.Substring(account.AccountNumber.Length - 2)} of GHS{amount} at {DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()} .Your current principal is GHS",
                TransCode = transcode,
                CreatedDate = DateTime.Now,
                PhoneNumber = phonenumber
            };
            _db.SmsLog.Add(smslog);
            _db.SaveChanges();
        }

        public void SendForApproval(string status, TransactionAccept transaction,string transcode)
        {
            TransCodeItems transcodeitems = new TransCodeItems
            {
                BranchId = transaction.transaction.BranchId,
                AccountId = transaction.transaction.AccountId,
                CreatedDate = transaction.transaction.CreatedDate,
                Credit = transaction.transaction.Credit,
                Debit = transaction.transaction.Debit,
                GeneralLedgerCodeId = transaction.transaction.GeneralLedgerCodeId,
                LedgerType = transaction.transaction.LedgerType,
                LoanId = transaction.transaction.LoanId,
                TransSource = transaction.transaction.TransSource,
                TransCode = transcode,
                SessionDate = transaction.transaction.SessionDate,
                Reference = transaction.transaction.Reference,
                Status = status
            };
            _db.TransCodeItems.Add(transcodeitems);
            _db.SaveChanges();
        }

        public void ConfigureAccountTransaction(string type, Account account, Transaction transaction=null, string transactioncode = null, TransCodeItems transcodeitems=null/*,string chequeNumber=null*/)
        {
            switch (type)
            {
                case "deposit":
                    Transaction trans1 = new Transaction();
                    //1st transaction entry
                    trans1.Debit = 0;
                    trans1.Credit =transcodeitems==null? Convert.ToDecimal(transaction?.Credit) : Convert.ToDecimal(transcodeitems?.Credit);
                    trans1.Balance = account.AvailableBalance;
                    trans1.GeneralLedgerCodeId = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(a => a.GeneralLedgerCodeId)).FirstOrDefault();
                    trans1.LedgerId =
                        (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId))
                        .FirstOrDefault();
                    trans1.LedgerType = "Account";
                    trans1.TransCode = transactioncode;
                    trans1.AccountId = account.AccountId;
                    trans1.BranchId = transcodeitems == null ? Convert.ToInt32(transaction?.BranchId) : Convert.ToInt32(transcodeitems?.BranchId);
                    trans1.SessionDate = transcodeitems == null ? transaction?.SessionDate: transcodeitems?.SessionDate;
                    trans1.Reference = transcodeitems == null ? transaction?.Reference : transcodeitems?.Reference;
                    trans1.TransSource = transcodeitems == null ? transaction?.TransSource : transcodeitems?.TransSource;
                    trans1.CreatedUserId = transcodeitems == null ? Convert.ToInt32(transaction?.CreatedUserId) : Convert.ToInt32(transcodeitems?.CreatedUserId);

                    decimal amount = transcodeitems == null ? Convert.ToDecimal(transaction?.Credit) : Convert.ToDecimal(transcodeitems?.Credit);
                    if (account.IndividualCustId != null)
                    {
                        LogSmsIndividual(account, transactioncode, "Credit", amount, transaction);
                    }
                    else if (account.CorporateCustId != null)
                    {
                        LogSmsCorporate(account, transactioncode, "Credit", amount, transaction);
                    }
                    else
                    {
                        LogSmsJoint(account, transactioncode, "Credit", amount, transaction);
                    }

                    Transaction trans2 = new Transaction();
                    //2nd transaction entry
                    trans2.Debit = transcodeitems == null ? Convert.ToDecimal(transaction?.Credit) : Convert.ToDecimal(transcodeitems?.Credit); 
                    trans2.Credit = 0;
                    trans2.Balance = account.AvailableBalance;
                    trans2.GeneralLedgerCodeId = transcodeitems == null ? transaction?.GeneralLedgerCodeId : transcodeitems?.GeneralLedgerCodeId;
                    trans2.LedgerId =
                        (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId))
                        .FirstOrDefault();
                    trans2.LedgerType = "Nominal";
                    trans2.TransCode =transactioncode;
                    trans2.AccountId = account.AccountId;
                    trans2.BranchId = transcodeitems == null ? Convert.ToInt32(transaction?.BranchId) : Convert.ToInt32(transcodeitems?.BranchId);
                    trans2.SessionDate = transcodeitems == null ? transaction?.SessionDate : transcodeitems?.SessionDate;
                    trans2.Reference = transcodeitems == null ? transaction?.Reference : transcodeitems?.Reference;
                    trans2.TransSource = transcodeitems == null ? transaction?.TransSource : transcodeitems?.TransSource;
                    trans2.CreatedUserId = transcodeitems == null ? Convert.ToInt32(transaction?.CreatedUserId) : Convert.ToInt32(transcodeitems?.CreatedUserId);

                    _db.Transactions.Add(trans1);
                    _db.SaveChanges();
                    _db.Transactions.Add(trans2);
                    _db.SaveChanges();
                    break;
                case "withdrawal":
                    Transaction trans3 = new Transaction();
                    //1st transaction entry
                    trans3.Debit = transcodeitems == null ? Convert.ToDecimal(transaction?.Debit) : Convert.ToDecimal(transcodeitems?.Debit); 
                    trans3.Credit = 0;
                    trans3.Balance = account.AvailableBalance;
                    trans3.GeneralLedgerCodeId = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(a => a.GeneralLedgerCodeId)).FirstOrDefault();
                    trans3.LedgerId =
                        (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId))
                        .FirstOrDefault();
                    trans3.LedgerType = "Account";
                    trans3.TransCode =transactioncode;
                    trans3.AccountId = account.AccountId;
                    trans3.BranchId = transcodeitems == null ? Convert.ToInt32(transaction?.BranchId) : Convert.ToInt32(transcodeitems?.BranchId);
                    trans3.SessionDate = transcodeitems == null ? transaction?.SessionDate : transcodeitems?.SessionDate;
                    trans3.Reference = transcodeitems == null ? transaction?.Reference : transcodeitems?.Reference;
                    trans3.CreatedUserId = transcodeitems == null ? Convert.ToInt32(transaction?.CreatedUserId) : Convert.ToInt32(transcodeitems?.CreatedUserId);
                    trans3.TransSource = transcodeitems == null ? transaction?.TransSource : transcodeitems?.TransSource;

                    if (account.IndividualCustId != null)
                    {
                        LogSmsIndividual(account, transactioncode, "Debit", Convert.ToDecimal(transcodeitems?.Debit), transaction);
                    }
                    else if (account.CorporateCustId != null)
                    {
                        LogSmsCorporate(account, transactioncode, "Debit", Convert.ToDecimal(transcodeitems?.Debit), transaction);
                    }
                    else
                    {
                        LogSmsJoint(account, transactioncode, "Debit", Convert.ToDecimal(transcodeitems?.Debit), transaction);
                    }

                    Transaction trans4 = new Transaction();
                    //2nd transaction entry
                    trans4.Debit = 0;
                    trans4.Credit = transcodeitems == null ? Convert.ToDecimal(transaction?.Debit) : Convert.ToDecimal(transcodeitems?.Debit); 
                    trans4.Balance = account.AvailableBalance;
                    trans4.GeneralLedgerCodeId = transcodeitems == null ? transaction?.GeneralLedgerCodeId : transcodeitems?.GeneralLedgerCodeId;
                    trans4.LedgerId =
                        (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId))
                        .FirstOrDefault();
                    trans4.LedgerType = "Nominal";
                    trans4.TransCode = transactioncode;
                    trans4.AccountId = account.AccountId;
                    trans4.BranchId = transcodeitems == null ? Convert.ToInt32(transaction?.BranchId) : Convert.ToInt32(transcodeitems?.BranchId);
                    trans4.SessionDate = transcodeitems == null ? transaction?.SessionDate : transcodeitems?.SessionDate;
                    trans4.Reference = transcodeitems == null ? transaction?.Reference : transcodeitems?.Reference;
                    trans4.CreatedUserId = transcodeitems == null ? Convert.ToInt32(transaction?.CreatedUserId) : Convert.ToInt32(transcodeitems?.CreatedUserId);
                    trans4.TransSource = transcodeitems == null ? transaction?.TransSource : transcodeitems?.TransSource;

                    _db.Transactions.Add(trans3);
                    _db.SaveChanges();
                    _db.Transactions.Add(trans4);
                    _db.SaveChanges();
                    break;
            }
        }

    }
}
