using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AliasWebApiCore.Services
{
    public class Scheduler:IScheduler
    {
        private readonly AppDbContext _db;

        public Scheduler(AppDbContext db)
        {
            _db = db;
        }

        

        //public Scheduler()
        //{
        //    RecurringJob.AddOrUpdate(() => DiscoverServices(null), Cron.MinuteInterval(1));
        //}

        [AutomaticRetry(Attempts = 0)]
        public async Task<bool> DiscoverServices(SavingsInterestModel model = null)
        {
            var services = _db.ServiceConfig.ToList();
            if (services.Count == 0) return true;
            foreach (ServiceConfig config in services)
            {
                switch (config.ServiceType.ToLower())
                {
                    case "sms":
                        if (config.Status == false)
                        {
                            RecurringJob.RemoveIfExists("Scheduler.Sms");
                        }
                        else
                        {
                            RecurringJob.AddOrUpdate(() => Sms(), config.CronSchedule);
                        }
                        break;
                    case "sweep":
                        if (config.Status == false)
                        {
                            RecurringJob.RemoveIfExists("Scheduler.Sweep");
                        }
                        else
                        {
                            RecurringJob.AddOrUpdate(() => Sweep(), config.CronSchedule);
                        }
                        break;
                    case "session manager":
                        if (config.Status == false)
                        {
                            RecurringJob.RemoveIfExists("Scheduler.SessionManager");
                        }
                        else
                        {
                            RecurringJob.AddOrUpdate(() => SessionManager(), config.CronSchedule);
                        }
                        break;
                    //case "fixed deposit interest accrued":
                    //    if (config.Status == false)
                    //    {
                    //        RecurringJob.RemoveIfExists("Scheduler.FixedDeposositInterestAccrued");
                    //    }
                    //    else
                    //    {
                    //        RecurringJob.AddOrUpdate(() => FixedDeposositInterestAccrued(), config.CronSchedule);
                    //    }
                    //    break;
                    //case "placement interest accrued":
                    //    if (config.Status == false)
                    //    {
                    //        RecurringJob.RemoveIfExists("Scheduler.PlacementInterestAccrued");
                    //    }
                    //    else
                    //    {
                    //        RecurringJob.AddOrUpdate(() => PlacementInterestAccrued(), config.CronSchedule);
                    //    }
                    //    break;
                    //case "savings interest":
                    //    if (config.Status == false)
                    //    {
                    //        RecurringJob.RemoveIfExists("Scheduler.SavingsInterest");
                    //    }
                    //    else
                    //    {
                    //        RecurringJob.AddOrUpdate(() => SavingsInterest(model), config.CronSchedule);
                    //    }
                    //    break;
                    //case "cot":
                    //    if (config.Status == false)
                    //    {
                    //        RecurringJob.RemoveIfExists("Scheduler.Cot");
                    //    }
                    //    else
                    //    {
                    //        RecurringJob.AddOrUpdate(() => Cot(model), config.CronSchedule);
                    //    }
                    //    BackgroundJob.Enqueue(() => Cot(model));
                    //    break;
                }
            }
            return true;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task<bool> Sms()
        {
            List<SmsLog> toSend = _db.SmsLog.Where(a => a.Status == false).ToList();
            SmsConfig smsconfig = _db.SmsConfig.FirstOrDefault(a => a.Status.ToLower().Contains("active"));
            if (smsconfig == null || toSend.Count == 0)
            {
                return true;
            }
            else
            {
                var httpClient = new HttpClient();
                StringBuilder sb = new StringBuilder();
                try
                {
                    sb.Append(smsconfig.HostUrl).Append("?&username=").Append(smsconfig.UserName)
                        .Append("&password=").Append(smsconfig.Password).Append("&source=").Append(smsconfig.Sender);
                    foreach (var tosend in toSend)
                    {
                        Account account = null;
                        FixedDeposit fdaccount = null;
                        if (tosend.Message.ToLower().Contains("principal"))
                        {
                            fdaccount = _db.FixedDeposit.FirstOrDefault(a => a.FixedDepositId == tosend.AccountId);
                            tosend.Message += string.Format("{0:0.##}", fdaccount?.FixedDepositPrincipal);
                        }
                        else
                        {
                            account = _db.Accounts.FirstOrDefault(a => a.AccountId == tosend.AccountId);
                            tosend.Message += string.Format("{0:0.##}", account?.AvailableBalance);
                        }
                        sb.Append("&destination=").Append(tosend.PhoneNumber).Append("&message=").Append(tosend.Message);
                        var json = await httpClient.GetStringAsync(sb.ToString());
                        var smsresponse = JsonConvert.DeserializeObject<SmsResponse>(json);
                        tosend.ResponseCode = smsresponse.Code;
                        tosend.Status = true;
                        tosend.SentDate = DateTime.Now;
                        _db.Entry(tosend).State = EntityState.Modified;
                    }
                    await _db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return true;
        }
        
        [AutomaticRetry(Attempts = 3)]
        public async Task<bool> Sweep()
        {
            IEnumerable<Sweep> standingOrder = _db.Sweeps.Where(a => a.Type.ToLower().Contains("standing")).ToList();
            IEnumerable<Sweep> sweep = _db.Sweeps.Where(a => a.Type.ToLower().Contains("sweep")).ToList();
            if (standingOrder.Any())
            {
                StartStandingOrder(standingOrder);
            }
            else if (sweep.Any())
            {
                StartSweep(sweep);
            }
            return true;
        }
        private void StartStandingOrder(IEnumerable<Sweep> _sweep)
        {
            foreach (Sweep sweep in _sweep)
            {
                if (DateTime.Now >= sweep.StartDate && DateTime.Now <= sweep.EndDate)
                {
                    if (DateTime.Now.Day == sweep.TransferDay)
                    {
                        var fromAccount = _db.Accounts.Find(sweep.FromAccountId);
                        var toAccount = _db.Accounts.Find(sweep.ToAccountId);
                        if (fromAccount != null && toAccount != null)
                        {
                            if (fromAccount.AvailableBalance >= sweep.Amount)
                            {
                                try
                                {
                                    fromAccount.TotalBalance -= sweep.Amount;
                                    fromAccount.AvailableBalance -= sweep.Amount;
                                    _db.Entry(fromAccount).State = EntityState.Modified;

                                    toAccount.TotalBalance += sweep.Amount;
                                    toAccount.AvailableBalance += sweep.Amount;
                                    _db.Entry(toAccount).State = EntityState.Modified;

                                    int fromglcode = (_db.AccountTypes
                                        .Where(a => a.AccountTypeId == fromAccount.AccountTypeId)
                                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                                    int toglcode = (_db.AccountTypes
                                        .Where(a => a.AccountTypeId == toAccount.AccountTypeId)
                                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();

                                    Transaction trans1 = new Transaction
                                    {
                                        GeneralLedgerCodeId = fromglcode,
                                        LedgerType = "Account",
                                        Balance = fromAccount.AvailableBalance,
                                        AccountId = fromAccount.AccountId,
                                        SessionDate =
                                        (_db.SessionManager.Where(a => a.Status.ToLower() == "active")
                                            .Select(b => b.SessionDate)).FirstOrDefault(),
                                        Debit = sweep.Amount,
                                        Credit = 0,
                                        TransSource = "Standing Order"
                                    };
                                    Transaction trans2 = new Transaction
                                    {
                                        GeneralLedgerCodeId = toglcode,
                                        LedgerType = "Account",
                                        Balance = toAccount.AvailableBalance,
                                        AccountId = toAccount.AccountId,
                                        SessionDate =
                                        (_db.SessionManager.Where(a => a.Status.ToLower() == "active")
                                            .Select(b => b.SessionDate)).FirstOrDefault(),
                                        Debit = 0,
                                        Credit = sweep.Amount,
                                        TransSource = "Standing Order"
                                    };
                                    _db.Transactions.Add(trans1);
                                    _db.Transactions.Add(trans2);
                                    _db.SaveChanges();
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                }
                            }
                            else
                            {
                                throw new Exception(
                                    "The available balance on the source account is lower than the sweep minimum balance.");
                            }
                        }
                        else
                        {
                            throw new Exception("The source and/or destination accounts may not exist.");
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        private void StartSweep(IEnumerable<Sweep> _sweep)
        {
            foreach (Sweep sweep in _sweep)
            {
                if (DateTime.Now >= sweep.StartDate && DateTime.Now <= sweep.EndDate)
                {
                    try
                    {
                        var fromAccount = _db.Accounts.Find(sweep.FromAccountId);
                        var toAccount = _db.Accounts.Find(sweep.ToAccountId);
                        if (fromAccount != null && toAccount != null)
                        {
                            if (fromAccount?.AvailableBalance >= sweep.MinimumBalance)
                            {

                                fromAccount.TotalBalance -= sweep.Amount;
                                fromAccount.AvailableBalance -= sweep.Amount;
                                _db.Entry(fromAccount).State = EntityState.Modified;

                                toAccount.TotalBalance += sweep.Amount;
                                toAccount.AvailableBalance += sweep.Amount;
                                _db.Entry(toAccount).State = EntityState.Modified;

                                int fromglcode = (_db.AccountTypes
                                    .Where(a => a.AccountTypeId == fromAccount.AccountTypeId)
                                    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                                int toglcode = (_db.AccountTypes
                                    .Where(a => a.AccountTypeId == toAccount.AccountTypeId)
                                    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();

                                Transaction trans1 = new Transaction
                                {
                                    GeneralLedgerCodeId = fromglcode,
                                    LedgerType = "Account",
                                    Balance = fromAccount.AvailableBalance,
                                    AccountId = fromAccount.AccountId,
                                    SessionDate =
                                    (_db.SessionManager.Where(a => a.Status.ToLower() == "active")
                                        .Select(b => b.SessionDate)).FirstOrDefault(),
                                    Debit = sweep.Amount,
                                    Credit = 0
                                };
                                Transaction trans2 = new Transaction
                                {
                                    GeneralLedgerCodeId = toglcode,
                                    LedgerType = "Account",
                                    Balance = toAccount.AvailableBalance,
                                    AccountId = toAccount.AccountId,
                                    SessionDate =
                                    (_db.SessionManager.Where(a => a.Status.ToLower() == "active")
                                        .Select(b => b.SessionDate)).FirstOrDefault(),
                                    Debit = 0,
                                    Credit = sweep.Amount
                                };
                                _db.Transactions.Add(trans1);
                                _db.Transactions.Add(trans2);
                                _db.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("The available balance on the source account is lower than the sweep minimum balance.");
                            }
                        }
                        else
                        {
                            throw new Exception("The source and/or destination accounts do not exist.");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                else
                {
                    return;
                }
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task<bool> SessionManager()
        {
            var currentSession = await _db.SessionManager.FirstOrDefaultAsync(a => a.Status.ToLower().Contains("active"));
            if (currentSession != null)
            {
                currentSession.Status = "closed";
                currentSession.ClosedDate = DateTime.Now;
                currentSession.ModifiedDate = DateTime.Now;
                _db.Entry(currentSession).State = EntityState.Modified;
            }
            SessionManager session = new SessionManager
            {
                SessionDate = DateTime.Now,
                Status = "active",
                CreatedDate = DateTime.Now,
            };
            _db.SessionManager.Add(session);
            //List<Account> accounts = _db.Accounts.Where(a => a.FixedDepositFundingSourceAccountId != null).ToList();
            //foreach (Account account in accounts)
            //{
            //    account.NewPeriod -= 1;
            //    _db.Entry(account).State = EntityState.Modified;
            //}
            await _db.SaveChangesAsync();
            return true;
        }

        //public async Task<bool> FixedDeposositInterestAccrued()
        //{
        //    var fdaccounts= _db.FixedDeposit.Where(a => a.Account.AccountStatus.ToLower().Contains("active") && a.FixedDepositFundingSourceAccountId != null)
        //        .GroupBy(x => x.FixedDepositTypeId).SelectMany(b => b).ToList();
        //     //var fdaccounts = _db.FixedDeposit.Where(a => a.AccountStatus.ToLower().Contains("active") && a.FixedDepositFundingSourceAccountId != null)
        //     //    .GroupBy(x => x.AccountTypeId).SelectMany(b => b).ToList();
        //    List<AccountDetails> accountDetails = new List<AccountDetails>();
        //    var currSessionDate = (_db.SessionManager.Where(a => a.Status.ToLower() == "active").Select(b => b.SessionDate)).FirstOrDefault();
        //    foreach (var fdaccount in fdaccounts)
        //    {
        //        var datediff = DateTime.Now - currSessionDate;
        //        if (accountDetails.Any(a => a.AccountId == fdaccount.FixedDepositId))
        //        {
        //            AccountDetails accdetails = accountDetails.FirstOrDefault(a => a.AccountId == fdaccount.FixedDepositId);
        //            if (accdetails != null)
        //                accdetails.DailyInterestSum +=
        //                    fdaccount.FixedDepositDailyInterest * (datediff.Days > 0 ? datediff.Days : 1);
        //        }
        //        accountDetails.Add(new AccountDetails
        //        {
        //            AccountId = fdaccount.FixedDepositId,
        //            DailyInterestSum = fdaccount.FixedDepositDailyInterest * (datediff.Days > 0 ? datediff.Days : 1)
        //        });
        //        fdaccount.InvIntLastAccruedDate = DateTime.Now;
        //        _db.Entry(fdaccount).State = EntityState.Modified;
        //    }
        //    foreach (var accountdetails in accountDetails)
        //    {

        //        var fdaccount = _db.FixedDeposit.Find(accountdetails.AccountId);
        //        Transaction trans1 = new Transaction
        //        {
        //            LedgerId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == fdaccount.AccountTypeId).Select(b => b.LedgerId))
        //                .FirstOrDefault(),
        //            LedgerType = "Nominal",
        //            GeneralLedgerCodeId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == fdaccount.AccountTypeId).Select(b => b.GLCodeForFDInterestAccrued)).FirstOrDefault(),
        //            Debit = 0,
        //            Credit = Convert.ToDecimal(accountdetails.DailyInterestSum),
        //            TransSource = "FixedDeposit Interest Accrued",
        //            AccountId = fdaccount.FixedDepositId
        //        };
        //        Transaction trans2 = new Transaction
        //        {
        //            LedgerId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == fdaccount.AccountTypeId).Select(b => b.LedgerId))
        //                .FirstOrDefault(),
        //            LedgerType = "Nominal",
        //            GeneralLedgerCodeId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == fdaccount.AccountTypeId).Select(b => b.GLCodeForFDInterestExpense)).FirstOrDefault(),
        //            Credit = 0,
        //            Debit = Convert.ToDecimal(accountdetails.DailyInterestSum),
        //            TransSource = "FixedDeposit Interest Accrued",
        //            AccountId = fdaccount.FixedDepositId
        //        };
        //        _db.Transactions.Add(trans1);
        //        _db.SaveChanges();
        //        _db.Transactions.Add(trans2);
        //        _db.SaveChanges();
        //    }
        //    return true;
        //}
        //public async Task<bool> PlacementInterestAccrued()
        //{
        //    var fdaccounts = _db.FixedDeposit.Where(a => a.AccountStatus.ToLower().Contains("active") && a.FixedDepositFundingSourceAccountId != null)
        //        .GroupBy(x => x.AccountTypeId).SelectMany(b => b).ToList();
        //    List<AccountDetails> accountDetails = new List<AccountDetails>();
        //    var currSessionDate = (_db.SessionManager.Where(a => a.Status.ToLower() == "active").Select(b => b.SessionDate)).FirstOrDefault();
        //    foreach (var fdaccount in fdaccounts)
        //    {
        //        var datediff = DateTime.Now - currSessionDate;
        //        if (accountDetails.Any(a => a.AccountId == fdaccount.FixedDepositId))
        //        {
        //            AccountDetails accdetails = accountDetails.FirstOrDefault(a => a.AccountId == fdaccount.FixedDepositId);
        //            if (accdetails != null)
        //                accdetails.DailyInterestSum +=
        //                    fdaccount.FixedDepositDailyInterest * (datediff.Days > 0 ? datediff.Days : 1);
        //        }
        //        accountDetails.Add(new AccountDetails
        //        {
        //            AccountId = fdaccount.FixedDepositId,
        //            DailyInterestSum = fdaccount.FixedDepositDailyInterest * (datediff.Days > 0 ? datediff.Days : 1)
        //        });
        //        fdaccount.InvIntLastAccruedDate = DateTime.Now;
        //        _db.Entry(fdaccount).State = EntityState.Modified;
        //    }
        //    foreach (var accountdetails in accountDetails)
        //    {

        //        var fdaccount = _db.FixedDeposit.Find(accountdetails.AccountId);
        //        Transaction trans1 = new Transaction
        //        {
        //            LedgerId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == fdaccount.AccountTypeId).Select(b => b.LedgerId))
        //                .FirstOrDefault(),
        //            LedgerType = "Nominal",
        //            GeneralLedgerCodeId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == fdaccount.AccountTypeId).Select(b => b.GLCodeForFDInterestAccrued)).FirstOrDefault(),
        //            Debit = Convert.ToDecimal(accountdetails.DailyInterestSum),
        //            Credit = 0,
        //            TransSource = "FixedDeposit Interest Accrued",
        //            AccountId = fdaccount.FixedDepositId
        //        };
        //        Transaction trans2 = new Transaction
        //        {
        //            LedgerId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == fdaccount.AccountTypeId).Select(b => b.LedgerId))
        //                .FirstOrDefault(),
        //            LedgerType = "Nominal",
        //            GeneralLedgerCodeId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == fdaccount.AccountTypeId).Select(b => b.GLCodeForFDInterestExpense)).FirstOrDefault(),
        //            Credit = Convert.ToDecimal(accountdetails.DailyInterestSum),
        //            Debit = 0,
        //            TransSource = "FixedDeposit Interest Accrued",
        //            AccountId = fdaccount.FixedDepositId
        //        };
        //        _db.Transactions.Add(trans1);
        //        _db.SaveChanges();
        //        _db.Transactions.Add(trans2);
        //        _db.SaveChanges();
        //    }
        //    return true;
        //}
        //[AutomaticRetry(Attempts = 3)]
        //public async Task<bool> SavingsInterest( SavingsInterestModel model=null)
        //{
        //    if (model == null)
        //    {
        //        return true;
        //    }
        //    List<AccountFormat> accountFormat = new List<AccountFormat>();
        //    foreach (int interesttype in model.InterestTypes)
        //    {
        //        accountFormat.Add(new AccountFormat { AccountTypeId = interesttype });
        //    }
        //    for (int i = 0; i < accountFormat.Count; i++)
        //    {
        //        accountFormat[i].AccountId =
        //        (_db.Accounts.Where(a => a.AccountTypeId == accountFormat[i].AccountTypeId)
        //            .Select(b => b.AccountId)).FirstOrDefault();
        //        accountFormat[i].SavingsInterestCalculationMethod =
        //        (_db.AccountTypes.Where(a => a.AccountTypeId == accountFormat[i].AccountTypeId)
        //            .Select(b => b.SavingsInterestCalculationMethod)).FirstOrDefault();
        //        accountFormat[i].SavingsInterestTypeId =
        //        (_db.AccountTypes.Where(a => a.AccountTypeId == accountFormat[i].AccountTypeId)
        //            .Select(b => b.SavingsInterestTypeId)).FirstOrDefault();
        //    }
        //    foreach (AccountFormat accountformat in accountFormat)
        //    {
        //        if (accountformat.SavingsInterestCalculationMethod.ToLower().Contains("closing"))
        //        {
        //            List<decimal> enddatebalcredit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.EndDate).Select(b => b.Credit)).ToList();
        //            List<decimal> enddatebaldebit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.EndDate).Select(b => b.Debit)).ToList();
        //            double percentage =
        //            (_db.Banktiers.Where(a => a.BanktiersId == accountformat.SavingsInterestTypeId)
        //                .Select(b => b.PercentageValue)).FirstOrDefault();
        //            List<decimal> amounts = new List<decimal>();
        //            for (int i = 0; i < enddatebalcredit.Count; i++)
        //            {
        //                var bal = (enddatebalcredit[i] - enddatebaldebit[i]);
        //                var updatedbal = (Convert.ToDecimal(percentage) / 100) * bal;
        //                amounts.Add(updatedbal);
        //            }
        //            accountformat.Amount = amounts;
        //        }
        //        else
        //        {
        //            var startdatebalcredit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.StartDate).Select(b => b.Credit)).ToList();
        //            var enddatebalcredit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.EndDate).Select(b => b.Credit)).ToList();
        //            var startdatebaldebit = (_db.Transactions.Where(a =>
        //                    a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                    && a.SessionDate <= model.StartDate && a.SessionDate <= model.EndDate).Select(b => b.Debit))
        //                .ToList();
        //            var enddatebaldebit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.EndDate).Select(b => b.Debit)).ToList();
        //            double percentage =
        //            (_db.Banktiers.Where(a => a.BanktiersId == accountformat.SavingsInterestTypeId)
        //                .Select(b => b.PercentageValue)).FirstOrDefault();
        //            List<decimal> credit = new List<decimal>();
        //            List<decimal> debit = new List<decimal>();
        //            for (int i = 0; i < startdatebalcredit.Count; i++)
        //            {
        //                credit.Add(Math.Min(startdatebalcredit[i], enddatebalcredit[i]));
        //            }
        //            for (int i = 0; i < startdatebaldebit.Count; i++)
        //            {
        //                credit.Add(Math.Min(startdatebaldebit[i], enddatebaldebit[i]));
        //            }
        //            List<decimal> amounts = new List<decimal>();
        //            for (int i = 0; i < credit.Count; i++)
        //            {
        //                var bal = (credit[i] - debit[i]);
        //                var updatedbal = (Convert.ToDecimal(percentage) / 100) * bal;
        //                amounts.Add(updatedbal);
        //            }
        //            accountformat.Amount = amounts;
        //        }
        //    }
        //    foreach (AccountFormat accountformat in accountFormat)
        //    {
        //        decimal totalamount = 0;
        //        foreach (decimal amt in accountformat.Amount)
        //        {
        //            TransCodeItems transitems = new TransCodeItems
        //            {
        //                Credit = Convert.ToDecimal(amt),
        //                AccountId = accountformat.AccountId,
        //                Debit = 0,
        //                GeneralLegderCodeId =
        //                (_db.AccountTypes.Where(a => a.AccountTypeId == accountformat.AccountTypeId)
        //                    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault()
        //            };
        //            _db.TransCodeItems.Add(transitems);
        //            totalamount += amt;
        //        }
        //        TransCodeItems transitems2 = new TransCodeItems
        //        {
        //            Credit = 0,
        //            AccountId = accountformat.AccountId,
        //            Debit = totalamount,
        //            GeneralLegderCodeId =
        //            (_db.Banktiers.Where(a => a.BanktiersId == accountformat.SavingsInterestTypeId)
        //                .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault()
        //        };
        //        _db.TransCodeItems.Add(transitems2);
        //        _db.SaveChanges();
        //    }
        //    return true;
        //}
        //[AutomaticRetry(Attempts = 3)]
        //public async Task<bool> Cot( SavingsInterestModel model=null)
        //{
        //    if (model == null)
        //    {
        //        return true;
        //    }
        //    List<AccountFormat> accountFormat = new List<AccountFormat>();
        //    foreach (int interesttype in model.InterestTypes)
        //    {
        //        accountFormat.Add(new AccountFormat { AccountTypeId = interesttype });
        //    }
        //    for (int i = 0; i < accountFormat.Count; i++)
        //    {
        //        accountFormat[i].AccountId =
        //        (_db.Accounts.Where(a => a.AccountTypeId == accountFormat[i].AccountTypeId)
        //            .Select(b => b.AccountId)).FirstOrDefault();
        //        accountFormat[i].SavingsInterestCalculationMethod =
        //        (_db.AccountTypes.Where(a => a.AccountTypeId == accountFormat[i].AccountTypeId)
        //            .Select(b => b.SavingsInterestCalculationMethod)).FirstOrDefault();
        //        accountFormat[i].SavingsInterestTypeId =
        //        (_db.AccountTypes.Where(a => a.AccountTypeId == accountFormat[i].AccountTypeId)
        //            .Select(b => b.SavingsInterestTypeId)).FirstOrDefault();
        //    }
        //    foreach (AccountFormat accountformat in accountFormat)
        //    {
        //        if (accountformat.SavingsInterestCalculationMethod.ToLower().Contains("closing"))
        //        {
        //            List<decimal> enddatebalcredit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.EndDate).Select(b => b.Credit)).ToList();
        //            List<decimal> enddatebaldebit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.EndDate).Select(b => b.Debit)).ToList();
        //            decimal chargeamount =
        //            (_db.Banktiers.Where(a => a.BanktiersId == accountformat.SavingsInterestTypeId)
        //                .Select(b => b.ChargeXAmountForActivityPeriod)).FirstOrDefault();
        //            List<decimal> amounts = new List<decimal>();
        //            for (int i = 0; i < enddatebalcredit.Count; i++)
        //            {
        //                var bal = (enddatebalcredit[i] - enddatebaldebit[i]);
        //                var updatedbal = bal - chargeamount;
        //                amounts.Add(updatedbal);
        //            }
        //            accountformat.Amount = amounts;
        //        }
        //        else
        //        {
        //            var startdatebalcredit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.StartDate).Select(b => b.Credit)).ToList();
        //            var enddatebalcredit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.EndDate).Select(b => b.Credit)).ToList();
        //            var startdatebaldebit = (_db.Transactions.Where(a =>
        //                    a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                    && a.SessionDate <= model.StartDate && a.SessionDate <= model.EndDate).Select(b => b.Debit))
        //                .ToList();
        //            var enddatebaldebit = (_db.Transactions.Where(a =>
        //                a.AccountId == accountformat.AccountId && a.LedgerType.ToLower().Contains("account")
        //                && a.SessionDate <= model.EndDate).Select(b => b.Debit)).ToList();
        //            decimal chargeamount =
        //            (_db.Banktiers.Where(a => a.BanktiersId == accountformat.SavingsInterestTypeId)
        //                .Select(b => b.ChargeXAmountForActivityPeriod)).FirstOrDefault();
        //            List<decimal> credit = new List<decimal>();
        //            List<decimal> debit = new List<decimal>();
        //            for (int i = 0; i < startdatebalcredit.Count; i++)
        //            {
        //                credit.Add(Math.Min(startdatebalcredit[i], enddatebalcredit[i]));
        //            }
        //            for (int i = 0; i < startdatebaldebit.Count; i++)
        //            {
        //                credit.Add(Math.Min(startdatebaldebit[i], enddatebaldebit[i]));
        //            }
        //            List<decimal> amounts = new List<decimal>();
        //            for (int i = 0; i < credit.Count; i++)
        //            {
        //                var bal = (credit[i] - debit[i]);
        //                var updatedbal = bal - chargeamount;
        //                amounts.Add(updatedbal);
        //            }
        //            accountformat.Amount = amounts;
        //        }
        //    }
        //    foreach (AccountFormat accountformat in accountFormat)
        //    {
        //        decimal totalamount = 0;
        //        foreach (decimal amt in accountformat.Amount)
        //        {
        //            TransCodeItems transitems = new TransCodeItems
        //            {
        //                Credit = Convert.ToDecimal(amt),
        //                AccountId = accountformat.AccountId,
        //                Debit = 0,
        //                GeneralLegderCodeId =
        //                (_db.AccountTypes.Where(a => a.AccountTypeId == accountformat.AccountTypeId)
        //                    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault()
        //            };
        //            _db.TransCodeItems.Add(transitems);
        //            totalamount += amt;
        //        }
        //        TransCodeItems transitems2 = new TransCodeItems
        //        {
        //            Credit = 0,
        //            AccountId = accountformat.AccountId,
        //            Debit = totalamount,
        //            GeneralLegderCodeId =
        //            (_db.Banktiers.Where(a => a.BanktiersId == accountformat.SavingsInterestTypeId)
        //                .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault()
        //        };
        //        _db.TransCodeItems.Add(transitems2);
        //        _db.SaveChanges();
        //    }
        //    return true;
        //}

    }

    public class SmsResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
    public class SavingsInterestModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int[] InterestTypes { get; set; }
    }

    public class AccountFormat
    {
        public int AccountId { get; set; }
        public int AccountTypeId { get; set; }
        public string SavingsInterestCalculationMethod { get; set; }
        public int? SavingsInterestTypeId { get; set; }
        public double PercentageValue { get; set; }
        public List<decimal> Amount { get; set; }
        public List<decimal> Balances { get; set; }
    }

}
