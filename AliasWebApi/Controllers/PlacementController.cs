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

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Placement")]
    [Authorize]
    public class PlacementController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ITransactionService _transService;

        public PlacementController(AppDbContext db, ITransactionService transService)
        {
            _db = db;
            _transService = transService;
        }

        [HttpGet("")]
        public IQueryable<FixedDeposit> GetPlAccounts()
        {
            return _db.FixedDeposit.Where(a=>a.InvestmentAlert!=null);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlAccount(int id)
        {
            FixedDeposit account =  (_db.FixedDeposit.Where(a=>a.FixedDepositId==id && a.InvestmentAlert!=null)).FirstOrDefault();
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpGet("all")]
        public IActionResult GetPlacements()
        {
            IEnumerable<FixedDeposit> plAccounts = _db.FixedDeposit.Where(a => a.InvestmentAlert!=null);
            List<AccountAllModel> accountAll = new List<AccountAllModel>();
            foreach (FixedDeposit account in plAccounts)
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

        [HttpGet("all/{id}")]
        public async Task<IActionResult> GetPlacementAll(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            FixedDeposit account = _db.FixedDeposit.FirstOrDefault(a => a.FixedDepositId == id && a.InvestmentAlert!=null);
            if (account != null)
            {
                AccountAllModel accountAll = new AccountAllModel
                {
                    AccountId = id,
                    AccountName = account.Account.IndividualCustId != null ? (_db.Individuals.Where(a => a.IndividualCustId == account.Account.IndividualCustId)
                        .Select(b => b.FirstName + " " + b.LastName)).FirstOrDefault() : (_db.Corporates.Where(c => c.CorporateCustId == account.Account.CorporateCustId)
                        .Select(g => g.CompanyName)).FirstOrDefault(),
                    AccountNumber = account.Account.AccountNumber,
                    //AvailableBalance = account.AvailableBalance,
                    //TotalBalance = account.TotalBalance,
                    BaseType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.Account.AccountTypeId).Select(b => b.BaseType)).FirstOrDefault(),
                    AccountStatus = account.Account.AccountStatus,
                    AccountType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.Account.AccountTypeId).Select(b => b.AccountType)).FirstOrDefault(),
                    //PurposeId = account.PurposeId
                };
                return Ok(accountAll);
            }
            return BadRequest($"Placement with id {id} not found");
        }

        [HttpPost("")]
        public async Task<IActionResult> PostPlacement([FromBody] FdModel model)
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
                    sourceAccount.AvailableBalance += Convert.ToInt32(model.FixedDeposit.FixedDepositPrincipal);
                    sourceAccount.TotalBalance += Convert.ToInt32(model.FixedDeposit.FixedDepositPrincipal);
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
                        model.FixedDeposit.Account.CreatedUserId, DateTime.Now, null, _db.Entry(model.FixedDeposit).Entity, sourceAccount);

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

        [HttpPost("topup")]
        public async Task<IActionResult> TopUp([FromBody] FdModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            FixedDeposit FixedDepositAccount = _db.FixedDeposit.Find(model.FixedDepositAccountId);
            var sourceAccount =
                _db.Accounts.FirstOrDefault(a => a.AccountId == FixedDepositAccount.FixedDepositFundingSourceAccountId);
            if (sourceAccount?.AvailableBalance < model.Amount)
            {
                return BadRequest("Insufficient available balance on servicing account");
            }

            var result = await ConfigureTrans("topup", model.Amount, model.BranchId, model.CreatedUserId, model.CreatedDate, model.Key, FixedDepositAccount,null);

            var accounttype = _db.AccountTypes.FirstOrDefault(a => a.AccountTypeId == FixedDepositAccount.Account.AccountTypeId);
            if (accounttype != null)
            {
                sourceAccount.AvailableBalance += model.Amount;
                sourceAccount.TotalBalance += model.Amount;
                FixedDepositAccount.FixedDepositPrincipal -= model.Amount;
                _db.Entry(sourceAccount).State = EntityState.Modified;
                _db.Entry(FixedDepositAccount).State = EntityState.Modified;

                double period = Convert.ToDouble(FixedDepositAccount.FixedDepositPeriod);
                DateTime? startdate = FixedDepositAccount.Account.OpenedDate;
                //DateTime? enddate = DateTime.Parse(startdate.ToString()).AddDays(period);
                //int newperiod = (DateTime.Parse(enddate.ToString()).Day - DateTime.Now.Day) < 0 ? ((DateTime.Parse(enddate.ToString()).Day - DateTime.Now.Day) / (-1)) : (DateTime.Parse(enddate.ToString()).Day - DateTime.Now.Day);
                int newperiod = (Convert.ToInt32(period) - DateTime.Now.Day) < 0
                    ? (Convert.ToInt32(period) - DateTime.Now.Day) * (-1)
                    : Convert.ToInt32(period) - DateTime.Now.Day;
                if (newperiod == 0)
                {
                    return BadRequest("Operations cannot be performed on the maturity date");
                }
                //int daysleft = DateTime.Parse(startdate.ToString()).Day - DateTime.Now.Day;
                var daysinyearforfdcalc = accounttype.NumberOfDaysInYearForFDInterestCal;
                FixedDepositAccount.FixedDepositDailyInterest =
                    ((FixedDepositAccount.FixedDepositInterestRate / 100) * FixedDepositAccount.FixedDepositPrincipal) / daysinyearforfdcalc;
                FixedDepositAccount.FixedDepositInterestAmount +=
                    (((FixedDepositAccount.FixedDepositInterestRate / 100) * FixedDepositAccount.FixedDepositPrincipal) / daysinyearforfdcalc) *
                    newperiod;
                FixedDepositAccount.FixedDepositMaturityAmount =
                    FixedDepositAccount.FixedDepositInterestAmount + FixedDepositAccount.FixedDepositPrincipal;
                _db.Entry(FixedDepositAccount).State = EntityState.Modified;
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
            FixedDeposit FixedDepositAccount = _db.FixedDeposit.Find(model.FixedDepositAccountId);
            var sourceAccount =
                _db.Accounts.FirstOrDefault(a => a.AccountId == FixedDepositAccount.FixedDepositFundingSourceAccountId);
            if (sourceAccount?.AvailableBalance < model.Amount)
            {
                return BadRequest("Insufficient available balance on servicing account");
            }

            Account fromAccount = await _db.Accounts.FindAsync(FixedDepositAccount.FixedDepositFundingSourceAccountId);
            if (model.Key.Contains("interest"))
            {
                FixedDepositAccount.FixedDepositInterestAccrued -= model.Amount;
            }
            else
            {
                FixedDepositAccount.FixedDepositPrincipal -= model.Amount;
            }
            if (FixedDepositAccount.FixedDepositPrincipal < 0)
            {
                return BadRequest("Partial Liquidation not successfull.Principal cannot be negative");
            }

            var accounttype = _db.AccountTypes.FirstOrDefault(a => a.AccountTypeId == FixedDepositAccount.Account.AccountTypeId);
            if (accounttype != null)
            {
                sourceAccount.AvailableBalance -= model.Amount;
                sourceAccount.TotalBalance -= model.Amount;
                _db.Entry(sourceAccount).State = EntityState.Modified;

                double period = Convert.ToDouble(FixedDepositAccount.FixedDepositPeriod);
                DateTime? startdate = FixedDepositAccount.Account.OpenedDate;
                //DateTime? enddate = DateTime.Parse(startdate.ToString()).AddDays(period);
                //int newperiod = DateTime.Parse(enddate.ToString()).Day - DateTime.Now.Day;
                int newperiod = (Convert.ToInt32(period) - DateTime.Now.Day) < 0
                    ? (Convert.ToInt32(period) - DateTime.Now.Day) * (-1)
                    : Convert.ToInt32(period) - DateTime.Now.Day;
                if (newperiod == 0)
                {
                    return BadRequest("Operations cannot be performed on the maturity date");
                }

                int daysleft = DateTime.Parse(startdate.ToString()).Day - DateTime.Now.Day;
                var daysinyearforfdcalc = accounttype.NumberOfDaysInYearForFDInterestCal;
                FixedDepositAccount.FixedDepositDailyInterest =
                    ((FixedDepositAccount.FixedDepositInterestRate / 100) * FixedDepositAccount.FixedDepositPrincipal) / daysinyearforfdcalc;
                FixedDepositAccount.FixedDepositInterestAmount -=
                    (((FixedDepositAccount.FixedDepositInterestRate / 100) * FixedDepositAccount.FixedDepositPrincipal) / daysinyearforfdcalc) *
                    newperiod;
                FixedDepositAccount.FixedDepositMaturityAmount =
                    FixedDepositAccount.FixedDepositInterestAmount + FixedDepositAccount.FixedDepositPrincipal;

                _db.Entry(FixedDepositAccount).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            var result = await ConfigureTrans("partialliquidation", model.Amount, model.BranchId, model.CreatedUserId, model.CreatedDate, model.Key, FixedDepositAccount,null);
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
            var result = await ConfigureTrans("preliquidation", Convert.ToDecimal(availableprincipal), model.BranchId, model.CreatedUserId, model.CreatedDate, model.Key, FixedDepositAccount,null);
            Account fromAccount = await _db.Accounts.FindAsync(FixedDepositAccount.FixedDepositFundingSourceAccountId);
            fromAccount.AvailableBalance -= Convert.ToInt32(FixedDepositAccount.FixedDepositPrincipal);
            fromAccount.TotalBalance -= Convert.ToInt32(FixedDepositAccount.FixedDepositPrincipal);
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
                var result = await ConfigureTrans("changeinterest", 0, 0, 0, null, reference, FixedDepositAccount,null);
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

        [NonAction]
        public async Task<bool> ConfigureTrans(string type, decimal amount = 0, int? branchid = 0, int? createduserid = 0, DateTime? createddate = null, string key = null, FixedDeposit fdaccount = null, Account account = null)
        {
            CommonSequence sequence = _db.CommonSequences.FirstOrDefault(a => a.Name.ToLower().Contains("transaction"));

            switch (type)
            {
                case "insertion":
                    var fromAccount = _db.Accounts.Find(fdaccount.FixedDepositFundingSourceAccountId);
                    int fromglcode = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == fromAccount.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    int toglcode = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == account.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();

                    var itranscode = _transService.GetTransCode("transaction");
                    Transaction trans0 = new Transaction
                    {
                        GeneralLedgerCodeId = fromglcode,
                        LedgerId =
                        (_db.AccountTypes.Where(a => a.AccountTypeId == fromAccount.AccountTypeId)
                        .Select(b => b.LedgerId)).FirstOrDefault(),
                        LedgerType = "Account",
                        Debit = 0,
                        Credit = Convert.ToDecimal(fdaccount.FixedDepositPrincipal),
                        AccountId = fromAccount.AccountId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = Convert.ToInt32(createduserid),
                        CreatedDate = DateTime.Now,
                        SessionDate = DateTime.Now,
                        Reference = "New Investment",
                        TransSource = "Placement",
                        TransCode = itranscode
                    };

                    Transaction trans1 = new Transaction
                    {
                        GeneralLedgerCodeId = toglcode,
                        LedgerId =
                            (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId))
                            .FirstOrDefault(),
                        LedgerType = "Account",
                        Credit = 0,
                        Debit = Convert.ToDecimal(fdaccount.FixedDepositPrincipal),
                        AccountId = fdaccount.FixedDepositId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = Convert.ToInt32(createduserid),
                        CreatedDate = DateTime.Now,
                        SessionDate = DateTime.Now,
                        Reference = "New Investment",
                        TransSource = "Placement",
                        TransCode = itranscode
                    };

                    _db.Transactions.Add(trans0);
                    _db.SaveChanges();
                    _db.Transactions.Add(trans1);
                    _db.SaveChanges();
                    break;
                case "topup":
                    var fromAccount1 = _db.Accounts.Find(fdaccount.FixedDepositFundingSourceAccountId);
                    int fromglcode1 = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == fromAccount1.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    int toglcode1 = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == account.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();

                    var ttranscode = _transService.GetTransCode("transaction");
                    Transaction trans2 = new Transaction
                    {
                        GeneralLedgerCodeId = fromglcode1,
                        LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == fromAccount1.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                        LedgerType = "Account",
                        Debit =0,
                        Credit = Convert.ToDecimal(amount/*fromAccount1.AvailableBalance*/),
                        AccountId = fromAccount1.AccountId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = Convert.ToInt32(createduserid),
                        CreatedDate = DateTime.Now,
                        SessionDate = DateTime.Now,
                        Reference = key,
                        TransSource = "Placement Top Up",
                        TransCode = ttranscode
                    };

                    Transaction trans3 = new Transaction
                    {
                        GeneralLedgerCodeId = toglcode1,
                        LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                        LedgerType = "Account",
                        Credit = 0,
                        Debit = Convert.ToDecimal(amount/*account.AvailableBalance*/),
                        AccountId = fdaccount.FixedDepositId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = Convert.ToInt32(createduserid),
                        CreatedDate = DateTime.Now,
                        SessionDate = DateTime.Now,
                        Reference = key,
                        TransSource = "Placement Top Up",
                        TransCode = ttranscode
                    };
                    //Log(fromAccount1, amount, "TopUp", ttranscode);
                    //Log(account, amount, "TopUp", ttranscode);
                    _db.Transactions.Add(trans2);
                    _db.SaveChanges();
                    _db.Transactions.Add(trans3);
                    _db.SaveChanges();
                    break;
                case "partialliquidation":
                    var fromAccount2 = _db.Accounts.Find(fdaccount.FixedDepositFundingSourceAccountId);
                    int fromglcode2 = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == account.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    int toglcode2 = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == account.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();

                    var partranscode = _transService.GetTransCode("transaction");
                    Transaction trans4 = new Transaction
                    {
                        GeneralLedgerCodeId = fromglcode2,
                        LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == fromAccount2.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                        LedgerType = "Account",
                        Credit = 0,
                        Debit = amount,
                        AccountId = fromAccount2.AccountId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = Convert.ToInt32(createduserid),
                        CreatedDate = DateTime.Now,
                        SessionDate = DateTime.Now,
                        Reference = key,
                        TransSource = "Placement Partial Liquidation",
                        TransCode = partranscode
                    };

                    Transaction trans5 = new Transaction
                    {
                        GeneralLedgerCodeId = toglcode2,
                        LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                        LedgerType = "Account",
                        Debit = 0,
                        Credit = amount,
                        AccountId = fdaccount.FixedDepositId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = Convert.ToInt32(createduserid),
                        CreatedDate = createddate,
                        Reference = key,
                        TransSource = "Placement Partial Liquidation",
                        TransCode = partranscode
                    };
                    //Log(fromAccount2, amount, "TopUp", partranscode);
                    //Log(account, amount, "TopUp", partranscode);
                    _db.Transactions.Add(trans4);
                    _db.SaveChanges();
                    _db.Transactions.Add(trans5);
                    _db.SaveChanges();
                    break;
                case "preliquidation":
                    var servicingaccount = _db.Accounts.Find(fdaccount.FixedDepositFundingSourceAccountId);
                    int fromglcode3 = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == servicingaccount.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();
                    int toglcode3 = (_db.AccountTypes
                        .Where(a => a.AccountTypeId == account.AccountTypeId)
                        .Select(b => b.GeneralLedgerCodeId)).FirstOrDefault();

                    var pretranscode = _transService.GetTransCode("transaction");
                    Transaction trans6 = new Transaction
                    {
                        GeneralLedgerCodeId = fromglcode3,
                        LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == servicingaccount.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                        LedgerType = "Account",
                        Credit = 0,
                        Debit = Convert.ToDecimal(servicingaccount.AvailableBalance),
                        AccountId = servicingaccount.AccountId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = Convert.ToInt32(createduserid),
                        CreatedDate = DateTime.Now,
                        SessionDate = DateTime.Now,
                        Reference = key,
                        TransSource = "Placement Pre Mature Liquidation",
                        TransCode = pretranscode
                    };

                    Transaction trans7 = new Transaction
                    {
                        GeneralLedgerCodeId = toglcode3,
                        LedgerId = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.LedgerId)).FirstOrDefault(),
                        LedgerType = "Account",
                        Debit = 0,
                        Credit = Convert.ToDecimal(account.AvailableBalance),
                        AccountId = fdaccount.FixedDepositId,
                        BranchId = Convert.ToInt32(branchid),
                        CreatedUserId = Convert.ToInt32(createduserid),
                        CreatedDate = DateTime.Now,
                        SessionDate = DateTime.Now,
                        Reference = key,
                        TransSource = "Placement Pre Mature Liquidation",
                        TransCode = pretranscode
                    };
                    //Log(servicingaccount, amount, "TopUp", pretranscode);
                    //Log(account,  amount, "TopUp", pretranscode);
                    _db.Transactions.Add(trans6);
                    _db.SaveChanges();
                    _db.Transactions.Add(trans7);
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


        //[NonAction]
        //private void Log(Account account, decimal amount, string operation, string ttranscode)
        //{
        //    if (account.IndividualCustId != null)
        //    {
        //        _transService.LogSmsIndividual(account, ttranscode, operation, amount, null);
        //    }
        //    else if (account.CorporateCustId != null)
        //    {
        //        _transService.LogSmsCorporate(account, ttranscode, operation, amount, null);
        //    }
        //    else
        //    {
        //        _transService.LogSmsJoint(account, ttranscode, operation, amount, null);
        //    }

        //}
    }


}
