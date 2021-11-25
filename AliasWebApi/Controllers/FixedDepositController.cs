using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using AliasWebApiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/FixedDeposit")]
    public class FixedDepositController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ITransactionService _transService;

        public FixedDepositController(AppDbContext db, ITransactionService transService)
        {
            _db = db;
            _transService = transService;
        }

        [HttpGet("all")]
        public IActionResult GetFdAccountsAll()
        {
            List<AccountAllModel> accountAll = new List<AccountAllModel>();
            for (int i = 0; i < _db.FixedDeposit.Count(); i++)
            {
                accountAll = (from a in _db.FixedDeposit
                              join b in _db.AccountTypes on a.Account.AccountTypeId equals
                                  b.AccountTypeId
                              select new AccountAllModel
                              {
                                  AccountId = a.FixedDepositId,
                                  AccountName = a.Account.IndividualCustId != null ? (_db.Individuals.Where(e => e.IndividualCustId == a.Account.IndividualCustId)
                           .Select(f => f.FirstName + " " + f.LastName)).FirstOrDefault() : (_db.Corporates.Where(f => f.CorporateCustId == a.Account.CorporateCustId)
                           .Select(g => g.CompanyName)).FirstOrDefault(),
                                  AccountNumber = a.Account.AccountNumber,
                                  //AvailableBalance = a.AvailableBalance,
                                  //TotalBalance = a.TotalBalance,
                                  BaseType = b.BaseType,
                                  AccountType = b.AccountType,
                                  AccountStatus = a.Account.AccountStatus,
                                  //PurposeId = a.PurposeId
                              }).ToList();
            }
            return Ok(accountAll);
        }

        [HttpGet("all/{id}")]
        public async Task<IActionResult> GetAccountAll(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            FixedDeposit fdaccount = await _db.FixedDeposit.FindAsync(id);
            if (fdaccount != null)
            {
                AccountAllModel accountAll = new AccountAllModel
                {
                    AccountId = id,
                    AccountName = fdaccount.Account.IndividualCustId != null ? (_db.Individuals.Where(a => a.IndividualCustId == fdaccount.Account.IndividualCustId)
                                    .Select(b => b.FirstName + " " + b.LastName)).FirstOrDefault() : (_db.Corporates.Where(c => c.CorporateCustId == fdaccount.Account.CorporateCustId)
                                    .Select(g => g.CompanyName)).FirstOrDefault(),
                    AccountNumber = fdaccount.Account.AccountNumber,
                    //AvailableBalance = fdaccount.AvailableBalance,
                    //TotalBalance = fdaccount.TotalBalance,
                    BaseType = (_db.AccountTypes.Where(a => a.AccountTypeId == fdaccount.Account.AccountTypeId).Select(b => b.BaseType)).FirstOrDefault(),
                    AccountStatus = fdaccount.Account.AccountStatus,
                    AccountType = (_db.AccountTypes.Where(a => a.AccountTypeId == fdaccount.Account.AccountTypeId).Select(b => b.AccountType)).FirstOrDefault(),
                   // PurposeId = fdaccount.PurposeId

                };
                return Ok(accountAll);
            }
            return BadRequest($"Account with id {id} not found");
        }

       [HttpGet("")]
        public IQueryable<FixedDeposit> GetFdAccounts()
        {
            return _db.FixedDeposit.Include(a=>a.Account);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFdAccount(int id)
        {
            FixedDeposit account = await _db.FixedDeposit.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpGet("{status}")]
        public IActionResult Account(string status)
        {

            IEnumerable<FixedDeposit> account = null;
            if (String.IsNullOrEmpty(status))
            {
                account = _db.FixedDeposit.Where(a => a.Account.AccountStatus.ToLower() == "pending").ToList();
            }
            else
            {
                account = _db.FixedDeposit.Where(a => a.Account.AccountStatus == status.ToLower()).ToList();
            }
            return Ok(account);
        }

        [HttpGet("Individuals/{id}")]
        public IActionResult GetFdAccountIndividual(int id)
        {
            IEnumerable<FixedDeposit> scAccounts = _db.FixedDeposit.Where(a => a.Account.IndividualCustId == id);
            List<AccountAllModel> accountAll = new List<AccountAllModel>();
            foreach (FixedDeposit account in scAccounts)
            {
                accountAll.Add(new AccountAllModel
                {
                    AccountId = account.FixedDepositId,
                    //AvailableBalance = account.AvailableBalance,
                    //TotalBalance = account.TotalBalance,
                    AccountNumber = account.Account.AccountNumber,
                    AccountName = account.Account.IndividualCustId != null ? (_db.Individuals.Where(e => e.IndividualCustId == account.Account.IndividualCustId)
                        .Select(f => f.FirstName + " " + f.LastName)).FirstOrDefault() : (_db.Corporates.Where(f => f.CorporateCustId == account.Account.CorporateCustId)
                        .Select(g => g.CompanyName)).FirstOrDefault(),
                    BaseType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.Account.AccountTypeId).Select(b => b.BaseType)).FirstOrDefault(),
                    AccountStatus = account.Account.AccountStatus,
                    //PurposeId = account.PurposeId

                });
            }
            return Ok(accountAll);
        }

        [HttpGet("Corporates/{id}")]
        public IActionResult GetAccountCorporate(int id)
        {
            IEnumerable<FixedDeposit> scAccounts = _db.FixedDeposit.Where(a => a.Account.CorporateCustId == id);
            List<AccountAllModel> accountAll = new List<AccountAllModel>();
            foreach (FixedDeposit account in scAccounts)
            {
                accountAll.Add(new AccountAllModel
                {
                    AccountId = account.FixedDepositId,
                    //AvailableBalance = account.AvailableBalance,
                    //TotalBalance = account.TotalBalance,
                    AccountNumber = account.Account.AccountNumber,
                    AccountName = account.Account.IndividualCustId != null ? (_db.Individuals.Where(e => e.IndividualCustId == account.Account.IndividualCustId)
                        .Select(f => f.FirstName + " " + f.LastName)).FirstOrDefault() : (_db.Corporates.Where(f => f.CorporateCustId == account.Account.CorporateCustId)
                        .Select(g => g.CompanyName)).FirstOrDefault(),
                    BaseType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.Account.AccountTypeId).Select(b => b.BaseType)).FirstOrDefault(),
                    AccountStatus = account.Account.AccountStatus,
                    //PurposeId = account.PurposeId

                });
            }
            return Ok(accountAll);
        }

        [HttpGet("JointCustomers/{id}")]
        public IActionResult GetAccountJoint(int id)
        {
            IEnumerable<FixedDeposit> scAccounts = _db.FixedDeposit.Where(a => a.Account.JointId == id);
            List<AccountAllModel> accountAll = new List<AccountAllModel>();
            foreach (FixedDeposit account in scAccounts)
            {
                accountAll.Add(new AccountAllModel
                {
                    AccountId = account.FixedDepositId,
                    //AvailableBalance = account.AvailableBalance,
                    //TotalBalance = account.TotalBalance,
                    AccountNumber = account.Account.AccountNumber,
                    AccountName = account.Account.IndividualCustId != null ? (_db.Individuals.Where(e => e.IndividualCustId == account.Account.IndividualCustId)
                        .Select(f => f.FirstName + " " + f.LastName)).FirstOrDefault() : (_db.Corporates.Where(f => f.CorporateCustId == account.Account.CorporateCustId)
                        .Select(g => g.CompanyName)).FirstOrDefault(),
                    BaseType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.Account.AccountTypeId).Select(b => b.BaseType)).FirstOrDefault(),
                    AccountStatus = account.Account.AccountStatus,
                    //PurposeId = account.PurposeId
                });
            }
            return Ok(accountAll);
        }

        [HttpPost("")]
        public async Task<IActionResult> PostFd([FromBody] FdModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_db.Accounts.Any(a => a.AccountId == model.FixedDeposit.FixedDepositFundingSourceAccountId))
                {
                    return BadRequest("Fixed Deposit Servicing Account does not exist");
                }
                Account sourceAccount =
                _db.Accounts.FirstOrDefault(a => a.AccountId == model.FixedDeposit.FixedDepositFundingSourceAccountId);
                if (sourceAccount?.AvailableBalance < model.FixedDeposit.FixedDepositPrincipal)
                {
                    return BadRequest("Insufficient available balance on servicing account");
                }
                else
                {
                    sourceAccount.AvailableBalance -= Convert.ToInt32(model.FixedDeposit.FixedDepositPrincipal);
                    sourceAccount.TotalBalance -= Convert.ToInt32(model.FixedDeposit.FixedDepositPrincipal);
                    _db.Entry(sourceAccount).State = EntityState.Modified;

                    var fdtype = _db.AccountTypes.FirstOrDefault(a =>
                        a.AccountTypeId == (_db.Entry(model.FixedDeposit.Account).Entity).AccountTypeId);

                    if (fdtype != null)
                    {
                        var daysinyearforfdcalc = fdtype.NumberOfDaysInYearForFDInterestCal;
                        model.FixedDeposit.FixedDepositDailyInterest =
                            ((model.FixedDeposit.FixedDepositInterestRate / 100) *
                             model.FixedDeposit.FixedDepositPrincipal) /
                            daysinyearforfdcalc;
                        model.FixedDeposit.FixedDepositInterestAmount =
                            (((model.FixedDeposit.FixedDepositInterestRate / 100) *
                              model.FixedDeposit.FixedDepositPrincipal) /
                             daysinyearforfdcalc) *
                            model.FixedDeposit.FixedDepositPeriod;
                        model.FixedDeposit.FixedDepositMaturityAmount =
                            model.FixedDeposit.FixedDepositInterestAmount + model.FixedDeposit.FixedDepositPrincipal;
                        _db.FixedDeposit.Add(model.FixedDeposit);
                        _db.SaveChanges();
                    }
                    else
                    {
                        return BadRequest(
                            $"Account Type with id {model.FixedDeposit.Account.AccountTypeId} does not exist");
                    }

                    var result = await ConfigureTrans("insertion", 0, model.FixedDeposit.Account.BranchId,
                        model.FixedDeposit.Account.CreatedUserId, DateTime.Now, null, _db.Entry(model.FixedDeposit).Entity,sourceAccount);

                    if (result)
                    {
                        return Ok(new
                        {
                            Status = "OK",
                            Message = "Successfully Added",
                            Output = "Fixed Deposit has been added"
                        });
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutFixedDeposit(int id, [FromBody] FixedDeposit model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != model.FixedDepositId)
            {
                return BadRequest();
            }

            model.Account.CreatedUserId = (_db.FixedDeposit.Where(a => a.FixedDepositId == id).Select(a => a.Account.CreatedUserId)).FirstOrDefault();
            model.Account.CreatedDate = (_db.FixedDeposit.Where(a => a.FixedDepositId == id).Select(a => a.Account.CreatedDate)).FirstOrDefault();
            _db.Entry(model).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FixedDepositExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [NonAction]
        private bool FixedDepositExists(int id)
        {
            return _db.FixedDeposit.Count(e => e.FixedDepositId == id) > 0;
        }

        [HttpPost("topup")]
        public async Task<IActionResult> TopUp([FromBody] FdModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            FixedDeposit fixedDepositAccount = _db.FixedDeposit.Find(model.FixedDepositAccountId);
            var sourceAccount =
                _db.Accounts.FirstOrDefault(a => a.AccountId == fixedDepositAccount.FixedDepositFundingSourceAccountId);
            if (sourceAccount == null)
            {
                return BadRequest("Servicing account not found");
            }
            if (sourceAccount?.AvailableBalance < model.Amount)
            {
                return BadRequest("Insufficient available balance on servicing account");
            }

            var result = await ConfigureTrans("topup", model.Amount,model.BranchId,model.CreatedUserId,model.CreatedDate,model.Key,fixedDepositAccount,null);

            var fdtype = _db.AccountTypes.FirstOrDefault(a => a.AccountTypeId == fixedDepositAccount.Account.AccountTypeId);
            if (fdtype != null)
            {
                sourceAccount.AvailableBalance -= model.Amount;
                sourceAccount.TotalBalance -= model.Amount;
                fixedDepositAccount.FixedDepositPrincipal += model.Amount;
                _db.Entry(sourceAccount).State = EntityState.Modified;
                _db.Entry(fixedDepositAccount).State = EntityState.Modified;

                double period = Convert.ToDouble(fixedDepositAccount.FixedDepositPeriod);
                DateTime? startdate = fixedDepositAccount.Account.OpenedDate;
                //DateTime? enddate = DateTime.Parse(startdate.ToString()).AddDays(period-1);
                //int newperiod = DateTime.Parse(enddate.ToString()).Day - DateTime.Now.Day;
                int newperiod = (Convert.ToInt32(period) - DateTime.Now.Day) < 0
                    ? (Convert.ToInt32(period) - DateTime.Now.Day) * (-1)
                    : Convert.ToInt32(period) - DateTime.Now.Day;
                if (newperiod == 0)
                {
                    return BadRequest("Operations cannot be performed on the maturity date");
                }

                var daysinyearforfdcalc = fdtype.NumberOfDaysInYearForFDInterestCal;
                fixedDepositAccount.FixedDepositDailyInterest =
                    ((fixedDepositAccount.FixedDepositInterestRate / 100) * fixedDepositAccount.FixedDepositPrincipal) / daysinyearforfdcalc;
                fixedDepositAccount.FixedDepositInterestAmount +=
                    (((fixedDepositAccount.FixedDepositInterestRate / 100) * fixedDepositAccount.FixedDepositPrincipal) / daysinyearforfdcalc) *
                    newperiod;
                fixedDepositAccount.FixedDepositMaturityAmount =
                    fixedDepositAccount.FixedDepositInterestAmount + fixedDepositAccount.FixedDepositPrincipal;
                _db.Entry(fixedDepositAccount).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            return Ok("TopUp successfull");
        }

        [HttpPost("partialliquidation")]
        public async Task<IActionResult> PartialLiquidation([FromBody] FdModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            FixedDeposit fixedDepositAccount = _db.FixedDeposit.Find(model.FixedDepositAccountId);
            var sourceAccount =
                _db.Accounts.FirstOrDefault(a => a.AccountId == fixedDepositAccount.FixedDepositFundingSourceAccountId);
            if (sourceAccount?.AvailableBalance < model.Amount)
            {
                return BadRequest("Insufficient available balance on servicing account");
            }

            Account fromAccount = await _db.Accounts.FindAsync(fixedDepositAccount.FixedDepositFundingSourceAccountId);
            if (model.Key.Contains("interest"))
            {
                fixedDepositAccount.FixedDepositInterestAccrued -= model.Amount;
            }
            else
            {
                fixedDepositAccount.FixedDepositPrincipal -= model.Amount;
            }
            if (fixedDepositAccount.FixedDepositPrincipal < 0)
            {
                return BadRequest("Partial Liquidation not successfull.Principal cannot be negative");
            }

            var accounttype = _db.AccountTypes.FirstOrDefault(a => a.AccountTypeId == fixedDepositAccount.Account.AccountTypeId);
            if (accounttype != null)
            {
                sourceAccount.AvailableBalance += model.Amount;
                sourceAccount.TotalBalance += model.Amount;
                _db.Entry(sourceAccount).State = EntityState.Modified;

                double period = Convert.ToDouble(fixedDepositAccount.FixedDepositPeriod);
                DateTime? startdate = fixedDepositAccount.Account.OpenedDate;
                //DateTime? enddate = DateTime.Parse(startdate.ToString()).AddDays(period);
                //int newperiod = DateTime.Parse(enddate.ToString()).Day - DateTime.Now.Day;
                int newperiod = (Convert.ToInt32(period) - DateTime.Now.Day) < 0
                    ? (Convert.ToInt32(period) - DateTime.Now.Day) * (-1)
                    : Convert.ToInt32(period) - DateTime.Now.Day;
                if (newperiod == 0)
                {
                    return BadRequest("Operations cannot be performed on the maturity date");
                }

                var daysinyearforfdcalc = accounttype.NumberOfDaysInYearForFDInterestCal;
               
                fixedDepositAccount.FixedDepositDailyInterest =
                    ((fixedDepositAccount.FixedDepositInterestRate / 100) * (fixedDepositAccount.FixedDepositPrincipal)) / daysinyearforfdcalc;
                fixedDepositAccount.FixedDepositInterestAmount -=
                    (((fixedDepositAccount.FixedDepositInterestRate / 100) * (fixedDepositAccount.FixedDepositPrincipal)) / daysinyearforfdcalc) *
                    newperiod;
                fixedDepositAccount.FixedDepositMaturityAmount =
                    fixedDepositAccount.FixedDepositInterestAmount + fixedDepositAccount.FixedDepositPrincipal;
                
                _db.Entry(fixedDepositAccount).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                
            }
            var result = await ConfigureTrans("partialliquidation", model.Amount, model.BranchId, model.CreatedUserId, model.CreatedDate, model.Key,fixedDepositAccount,null);

            return Ok("Partial Liquidation successfull");
        }

        [HttpPost("preliquidation")]
        public async Task<IActionResult> PreLiquidation([FromBody] FdModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            FixedDeposit FixedDepositAccount = _db.FixedDeposit.Find(model.FixedDepositAccountId);
            var availableprincipal = FixedDepositAccount.FixedDepositPrincipal;
            //var result = await ConfigureTrans("preliquidation", FixedDepositAccount, Convert.ToDecimal(availableprincipal), model.BranchId, model.CreatedUserId,model.CreatedDate, model.Key);
            Account fromAccount = await _db.Accounts.FindAsync(FixedDepositAccount.FixedDepositFundingSourceAccountId);
            fromAccount.AvailableBalance += Convert.ToInt32(FixedDepositAccount.FixedDepositPrincipal);
            fromAccount.TotalBalance += Convert.ToInt32(FixedDepositAccount.FixedDepositPrincipal);
            _db.Entry(fromAccount).State = EntityState.Modified;
            FixedDepositAccount.FixedDepositPrincipal = 0;
            

            var accounttype = _db.AccountTypes.FirstOrDefault(a => a.AccountTypeId == FixedDepositAccount.Account.AccountTypeId);
            if (accounttype != null)
            {
                FixedDepositAccount.FixedDepositDailyInterest = 0;
                FixedDepositAccount.FixedDepositInterestAmount = 0;
                FixedDepositAccount.FixedDepositMaturityAmount = 0;
                FixedDepositAccount.Account.AccountStatus = "Expired";
               
                    _db.Entry(FixedDepositAccount).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
            }
            var result = await ConfigureTrans("preliquidation",Convert.ToDecimal(availableprincipal), model.BranchId, model.CreatedUserId, model.CreatedDate, model.Key,FixedDepositAccount,null);

            return Ok("PreMature Liquidation successfull");
        }

        [HttpPost("changeinterestrate")]
        public async Task<IActionResult> ChangeInterestRate([FromBody] FdModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            FixedDeposit FixedDepositAccount = _db.FixedDeposit.Find(model.FixedDepositAccountId);
            if (FixedDepositAccount == null)
            {
                return BadRequest("Account does not exist");
            }
           
            var accounttype = _db.AccountTypes.FirstOrDefault(a => a.AccountTypeId == FixedDepositAccount.Account.AccountTypeId);
            if (accounttype != null)
            {
                double period = Convert.ToDouble(FixedDepositAccount.FixedDepositPeriod);
                DateTime? startdate = FixedDepositAccount.Account.OpenedDate;
                DateTime? enddate = DateTime.Parse(startdate.ToString()).AddDays(period);
                int newperiod = DateTime.Parse(enddate.ToString()).Day - DateTime.Now.Day;

                int daysleft = DateTime.Parse(startdate.ToString()).Day - DateTime.Now.Day;
                var daysinyearforfdcalc = accounttype.NumberOfDaysInYearForFDInterestCal;
                FixedDepositAccount.FixedDepositDailyInterest =
                    ((FixedDepositAccount.FixedDepositInterestRate / 100) * FixedDepositAccount.FixedDepositPrincipal) / daysinyearforfdcalc;
                FixedDepositAccount.FixedDepositInterestAmount -=
                    (((FixedDepositAccount.FixedDepositInterestRate / 100) * FixedDepositAccount.FixedDepositPrincipal) / daysinyearforfdcalc) *
                    newperiod;
                FixedDepositAccount.FixedDepositMaturityAmount =
                    FixedDepositAccount.FixedDepositInterestAmount + FixedDepositAccount.FixedDepositPrincipal;
                FixedDepositAccount.FixedDepositInterestRate = model.NewInterestRate;

                _db.Entry(FixedDepositAccount).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                var reference =
                    $"Changed Interest Rate from {FixedDepositAccount.FixedDepositInterestRate} to {model.NewInterestRate}";
                var result = await ConfigureTrans("changeinterest", 0,0, 0,null, reference,FixedDepositAccount,null);
            }
            return Ok("Interest Rate Change successfull");
        }

        [HttpPost("adjustrollover")]
        public async Task<IActionResult> AdjustRollover([FromBody] FdModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            FixedDeposit FixedDepositAccount = _db.FixedDeposit.Find(model.FixedDepositAccountId);
            if (FixedDepositAccount != null)
            {
                FixedDepositAccount.RolloverInterest = model.RolloverInterest;
                FixedDepositAccount.RolloverPrincipal = model.RolloverPrincipal;
                FixedDepositAccount.NewInterestRate = model.NewFdInterestRate;
                FixedDepositAccount.NewPeriod = model.NewPeriod;
                _db.Entry(FixedDepositAccount).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return Ok("Rollover Adjustment Successfull");
            }
            return BadRequest("Account does not exist");
        }

        //[HttpPost("AccruedInterest")]
        //public IActionResult InterestAccrued()
        //{
        //    var fdaccounts = _db.FixedDeposit.Where(a => a.AccountStatus.ToLower().Contains("active") && a.FixedDepositFundingSourceAccountId!=null)
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
        //            DailyInterestSum = fdaccount.FixedDepositDailyInterest * (datediff.Days>0? datediff.Days:1)
        //        });
        //        fdaccount.InvIntLastAccruedDate=DateTime.Now;
        //        _db.Entry(fdaccount).State = EntityState.Modified;
        //    }
        //    foreach (var accountdetails in accountDetails)
        //    {

        //        var account = _db.Accounts.Find(accountdetails.AccountId);
        //        Transaction trans1 = new Transaction
        //        {
        //            LedgerId= (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId))
        //                .FirstOrDefault(),
        //            LedgerType = "Nominal",
        //            GeneralLedgerCodeId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == account.AccountTypeId).Select(b => b.GLCodeForFDInterestAccrued)).FirstOrDefault(),
        //            Debit = 0,
        //            Credit = Convert.ToDecimal(accountdetails.DailyInterestSum),
        //            TransSource = "FixedDeposit Interest Accrued",
        //            AccountId = account.AccountId
        //        };
        //        Transaction trans2 = new Transaction
        //        {
        //            LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId))
        //                .FirstOrDefault(),
        //            LedgerType = "Nominal",
        //            GeneralLedgerCodeId = (_db.FixedDepositTypes.Where(a => a.FixedDepositTypeId == account.AccountTypeId).Select(b => b.GLCodeForFDInterestExpense)).FirstOrDefault(),
        //            Credit = 0,
        //            Debit = Convert.ToDecimal(accountdetails.DailyInterestSum),
        //            TransSource = "FixedDeposit Interest Accrued",
        //            AccountId = account.AccountId
        //        };
        //        _db.Transactions.Add(trans1);
        //        _db.SaveChanges();
        //        _db.Transactions.Add(trans2);
        //        _db.SaveChanges();
        //    }
        //    return Ok();
        //}

        [NonAction]
        public async Task<bool> ConfigureTrans( string type ,decimal amount=0,int? branchid=0,int? createduserid=0,DateTime? createddate=null,string key=null,FixedDeposit fdaccount=null,Account account=null)
        {
            switch (type)
            {
                case "insertion":
                    int fromglcode = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == account.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    int toglcode = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == fdaccount.Account.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    fdaccount.Account.AccountStatus = "Inactive";
                    _db.Entry(fdaccount).State = EntityState.Modified;
                    var itranscode = _transService.GetTransCode("transaction");
                    Transaction trans0 = new Transaction
                    {
                        GeneralLedgerCodeId = fromglcode,
                        LedgerId =
                        (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId)
                        .Select(b => b.LedgerId)).FirstOrDefault(),
                        LedgerType = "Account",
                        Debit = Convert.ToDecimal(fdaccount.FixedDepositPrincipal),
                        Credit = 0,
                        AccountId = account?.AccountId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = createduserid,
                        CreatedDate = DateTime.Now,
                        SessionDate = DateTime.Now,
                        Reference = "New Investment",
                        TransSource = "Fixed Deposit",
                        TransCode = itranscode
                    };

                    Transaction trans1 = new Transaction
                    {
                        GeneralLedgerCodeId = toglcode,
                        LedgerId =
                            (_db.AccountTypes.Where(a => a.AccountTypeId == fdaccount.Account.AccountTypeId).Select(b => b.LedgerId))
                            .FirstOrDefault(),
                        LedgerType = "Account",
                        Credit = Convert.ToDecimal(fdaccount.FixedDepositPrincipal),
                        Debit = 0,
                        AccountId = fdaccount.AccountId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = createduserid,
                        CreatedDate = DateTime.Now,
                        SessionDate = DateTime.Now,
                        Reference = "New Investment",
                        TransSource = "Fixed Deposit",
                        TransCode = itranscode
                    };

                    _db.Transactions.Add(trans0);
                    _db.SaveChanges();
                    _db.Transactions.Add(trans1);
                    _db.SaveChanges();
                    break;
                case "topup":
                    var fromAccount1 = _db.Accounts.Find(fdaccount.FixedDepositFundingSourceAccountId);
                    int gl1= (_db.AccountTypes
                        .Where(a => a.AccountTypeId == fromAccount1.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    //int fromglcode1 = (_db.AccountTypes
                    //    .Where(a => a.AccountTypeId == fromAccount1.AccountTypeId)
                    //    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    //int toglcode1 = (_db.AccountTypes
                    //    .Where(a => a.AccountTypeId == fromAccount1.AccountTypeId)
                    //    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();

                    var ttranscode = _transService.GetTransCode("transaction");
                    //Transaction trans2 = new Transaction
                    //{
                    //    GeneralLedgerCodeId = gl1,
                    //    LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == fromAccount1.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                    //    LedgerType = "Account",
                    //    Debit = Convert.ToDecimal(amount),
                    //    Credit = 0,
                    //    AccountId = fromAccount1.AccountId,
                    //    BranchId = Convert.ToInt32(branchid),
                    //    CreatedUserId =createduserid ,
                    //    CreatedDate = DateTime.Now,
                    //    SessionDate = DateTime.Now,
                    //    Reference = key,
                    //    TransSource = "Fixed Deposit Top Up",
                    //    TransCode = ttranscode
                    //};

                    //Transaction trans3 = new Transaction
                    //{
                    //    GeneralLedgerCodeId = gl1,
                    //    LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                    //    LedgerType = "Account",
                    //    Credit = Convert.ToDecimal(amount/*account.AvailableBalance*/),
                    //    Debit = 0,
                    //    AccountId = fdaccount.FixedDepositId,
                    //    BranchId = Convert.ToInt32(branchid),
                    //    CreatedUserId =createduserid,
                    //    CreatedDate = DateTime.Now,
                    //    SessionDate = DateTime.Now,
                    //    Reference = key,
                    //    TransSource = "Fixed Deposit Top Up",
                    //    TransCode = ttranscode
                    //};
                    //Log(amount, fromAccount1, "TopUp",ttranscode);
                    //Log(amount,account, "TopUp", ttranscode,"fd");
                    //_db.Transactions.Add(trans2);
                    //_db.SaveChanges();
                    //_db.Transactions.Add(trans3);
                    _db.SaveChanges();
                    break;
                case "partialliquidation":
                    var fromAccount2 = _db.Accounts.Find(fdaccount.FixedDepositFundingSourceAccountId);
                    int gl2 = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == account.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    //int fromglcode2 = (_db.AccountTypes
                    //    .Where(a => a.AccountTypeId == account.AccountTypeId)
                    //    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    //int toglcode2 = (_db.AccountTypes
                    //    .Where(a => a.AccountTypeId == account.AccountTypeId)
                    //    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();

                    var partranscode = _transService.GetTransCode("transaction");
                    //Transaction trans4 = new Transaction
                    //{
                    //    GeneralLedgerCodeId = gl2,
                    //    LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == fromAccount2.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                    //    LedgerType = "Account",
                    //    Credit = amount,
                    //    Debit = 0,
                    //    AccountId = fromAccount2.AccountId,
                    //    BranchId = Convert.ToInt32(branchid),
                    //    CreatedUserId = createduserid,
                    //    CreatedDate = DateTime.Now,
                    //    SessionDate = DateTime.Now,
                    //    Reference = key,
                    //    TransSource = "Fixed Deposit Partial Liquidation",
                    //    TransCode = partranscode
                    //};

                    //Transaction trans5 = new Transaction
                    //{
                    //    GeneralLedgerCodeId = gl2,
                    //    LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                    //    LedgerType = "Account",
                    //    Debit = amount,
                    //    Credit = 0,
                    //    AccountId = account.AccountId,
                    //    BranchId = Convert.ToInt32(branchid),
                    //    CreatedUserId = createduserid,
                    //    CreatedDate = createddate,
                    //    Reference = key,
                    //    TransSource = "Fixed Deposit Partial Liquidation",
                    //    TransCode = partranscode
                    //};
                    //Log( amount,fromAccount2, "Partial Liquidation", partranscode);
                    //Log(amount,account, "Partial Liquidation", partranscode,"fd");
                    //_db.Transactions.Add(trans4);
                    //_db.SaveChanges();
                    //_db.Transactions.Add(trans5);
                    _db.SaveChanges();
                    break;
                case "preliquidation":
                    var servicingaccount = _db.Accounts.Find(fdaccount.FixedDepositFundingSourceAccountId);
                    int gl3= (_db.AccountTypes
                        .Where(a => a.AccountTypeId == servicingaccount.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    //int fromglcode3 = (_db.AccountTypes
                    //    .Where(a => a.AccountTypeId == servicingaccount.AccountTypeId)
                    //    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    //int toglcode3 = (_db.AccountTypes
                    //    .Where(a => a.AccountTypeId == account.AccountTypeId)
                    //    .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();

                    var pretranscode = _transService.GetTransCode("transaction");
                    //Transaction trans6 = new Transaction
                    //{
                    //    GeneralLedgerCodeId = gl3,
                    //    LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == servicingaccount.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                    //    LedgerType = "Account",
                    //    Credit = Convert.ToDecimal(servicingaccount.AvailableBalance),
                    //    Debit = 0,
                    //    AccountId = servicingaccount.AccountId,
                    //    BranchId = Convert.ToInt32(branchid),
                    //    CreatedUserId = createduserid,
                    //    CreatedDate = DateTime.Now,
                    //    SessionDate = DateTime.Now,
                    //    Reference = key,
                    //    TransSource = "Fixed Deposit Pre Mature Liquidation",
                    //    TransCode = pretranscode
                    //};

                    //Transaction trans7 = new Transaction
                    //{
                    //    GeneralLedgerCodeId = gl3,
                    //    LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                    //    LedgerType = "Account",
                    //    Debit = Convert.ToDecimal(account.AvailableBalance),
                    //    Credit = 0,
                    //    AccountId = fdaccount.FixedDepositId,
                    //    BranchId = Convert.ToInt32(branchid),
                    //    CreatedUserId = createduserid,
                    //    CreatedDate = DateTime.Now,
                    //    SessionDate = DateTime.Now,
                    //    Reference = key,
                    //    TransSource = "Fixed Deposit Pre Mature Liquidation",
                    //    TransCode = pretranscode
                    //};
                    //Log(amount,servicingaccount, "Pre Mature Liquidation", pretranscode,null);
                    //Log(amount,account, "TopUp", pretranscode,"fd");
                    //_db.Transactions.Add(trans6);
                    //_db.SaveChanges();
                    //_db.Transactions.Add(trans7);
                    _db.SaveChanges();
                    break;
                case "changeinterest":
                    //var servicingaccount2 = _db.Accounts.Find(account.FixedDepositFundingSourceAccountId);
                    var citranscode = _transService.GetTransCode("transaction");
                    Transaction trans8 = new Transaction
                    {
                        LedgerId =
                            (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId))
                            .FirstOrDefault(),
                        AccountId = fdaccount.FixedDepositId,
                        Reference = key,
                        TransCode = citranscode,
                        TransSource = "Interest Change"
                    };
                    break;
            }

            return true;
        }

        [NonAction]
        private void Log(decimal amount,Account account,string operation, string ttranscode,string accounttype=null)
        {
            if (account.IndividualCustId != null)
            {
                if(accounttype=="fd")
                    _transService.LogSmsIndividual(account, ttranscode, operation, amount,null,accounttype);
            }
            else if (account.CorporateCustId != null)
            {
                if (accounttype == "fd")
                    _transService.LogSmsCorporate(account,ttranscode, operation,amount,null,accounttype);
            }
            else
            {
                if (accounttype == "fd")
                    _transService.LogSmsJoint(account,ttranscode, operation, amount, null,accounttype);
            }

        }

    }
    
    
}