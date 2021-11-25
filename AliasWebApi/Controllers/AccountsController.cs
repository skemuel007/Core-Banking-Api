using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using AliasWebApiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.KeyVault;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class AccountsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ITransactionService _tranService;

        public AccountsController(AppDbContext db, ITransactionService tranService)
        {
            _db = db;
            _tranService = tranService;
        }

        // GET: api/Accounts/all
        [HttpGet("accounts/all")]
        public IActionResult GetAccountsAll()
        {
            List<AccountAllModel> accountAll = new List<AccountAllModel>();
            for (int i = 0; i < _db.Accounts.Count(); i++)
            {
                accountAll = (from a in _db.Accounts
                    join b in _db.AccountTypes on a.AccountTypeId equals
                        b.AccountTypeId
                              select new AccountAllModel
                    {
                        AccountId = a.AccountId,
                        AccountName = a.IndividualCustId!=null? (_db.Individuals.Where(e => e.IndividualCustId == a.IndividualCustId)
                           .Select(f => f.FirstName + " " + f.LastName)).FirstOrDefault() : (_db.Corporates.Where(f=>f.CorporateCustId==a.CorporateCustId)
                           .Select(g=>g.CompanyName)).FirstOrDefault() ,
                        AccountNumber = a.AccountNumber,
                        AvailableBalance = a.AvailableBalance,
                        TotalBalance = a.TotalBalance,
                        BaseType = b.BaseType,
                        AccountType = b.AccountType,
                        AccountStatus =a.AccountStatus,
                        PurposeId = a.PurposeId
                    }).ToList();
            }
            return Ok(accountAll);
        }

        // GET: api/Accounts/all/2
        [HttpGet("accounts/all/{id}")]
        public async Task<IActionResult> GetAccountAll(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Account account = await _db.Accounts.FindAsync(id);
            if (account != null)
            {
                AccountAllModel accountAll = new AccountAllModel
                {
                    AccountId = id,
                    AccountName = account.IndividualCustId != null ? (_db.Individuals.Where(a => a.IndividualCustId == account.IndividualCustId)
                                    .Select(b => b.FirstName + " " + b.LastName)).FirstOrDefault() : (_db.Corporates.Where(c => c.CorporateCustId == account.CorporateCustId)
                                    .Select(g => g.CompanyName)).FirstOrDefault(),
                    AccountNumber = account.AccountNumber,
                    AvailableBalance = account.AvailableBalance,
                    TotalBalance = account.TotalBalance,
                    BaseType = (_db.AccountTypes.Where(a=>a.AccountTypeId==account.AccountTypeId).Select(b=>b.BaseType)).FirstOrDefault(),
                    AccountStatus = account.AccountStatus,
                    AccountType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.AccountType)).FirstOrDefault(),
                    PurposeId = account.PurposeId

                };
                return Ok(accountAll);
            }
            return BadRequest($"Account with id {id} not found");
        }

        // GET: api/Accounts
        [HttpGet("accounts")]
        public IQueryable<Account> GetAccounts()
        {
            return _db.Accounts;
        }

        //// GET: api/Accounts/sc
        //[HttpGet("accounts/sc")]
        //public IActionResult GetSavingsAndAccounts()
        //{
        //    IEnumerable<Account> scAccounts= _db.Accounts.Where(a => a.FixedDepositFundingSourceAccountId == null);
        //    List<AccountAllModel> accountAll = new List<AccountAllModel>();
        //    foreach (Account account in scAccounts)
        //    {
        //        accountAll.Add(new AccountAllModel
        //        {
        //            AccountId = account.AccountId,
        //            AvailableBalance = account.AvailableBalance,
        //            TotalBalance = account.TotalBalance,
        //            AccountNumber = account.AccountNumber,
        //            AccountName = account.IndividualCustId != null ? (_db.Individuals.Where(e => e.IndividualCustId == account.IndividualCustId)
        //                            .Select(f => f.FirstName + " " + f.LastName)).FirstOrDefault() : (_db.Corporates.Where(f => f.CorporateCustId == account.CorporateCustId)
        //                            .Select(g => g.CompanyName)).FirstOrDefault(),
        //            BaseType = (_db.AccountTypes.Where(a=>a.AccountTypeId==account.AccountTypeId).Select(b=>b.BaseType)).FirstOrDefault(),
        //            AccountStatus = account.AccountStatus,
        //            PurposeId = account.PurposeId

        //        });
        //    }
        //    return Ok(accountAll);
        //}

        // GET: api/Accounts/5
        [HttpGet("accounts/{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            Account account = await _db.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }

        //GET: api/Accounts/Active
        [HttpGet("accounts/{status}")]
        public IActionResult Account(string status)
        {

            IEnumerable<Account> account = null;
            if (String.IsNullOrEmpty(status))
            {
                account = _db.Accounts.Where(a => a.AccountStatus.ToLower() == "pending").ToList();
            }
            else
            {
                account = _db.Accounts.Where(a => a.AccountStatus == status.ToLower()).ToList();
            }
            return Ok(account);

        }

        // GET: api/Accounts/Individuals/5
        [HttpGet("accounts/Individuals/{id}")]
        public IActionResult GetAccountIndividual(int id)
        {
            IEnumerable<Account> scAccounts = _db.Accounts.Where(a => a.IndividualCustId == id);
            List<AccountAllModel> accountAll = new List<AccountAllModel>();
            foreach (Account account in scAccounts)
            {
                accountAll.Add(new AccountAllModel
                {
                    AccountId = account.AccountId,
                    AvailableBalance = account.AvailableBalance,
                    TotalBalance = account.TotalBalance,
                    AccountNumber = account.AccountNumber,
                    AccountName = account.IndividualCustId != null ? (_db.Individuals.Where(e => e.IndividualCustId == account.IndividualCustId)
                        .Select(f => f.FirstName + " " + f.LastName)).FirstOrDefault() : (_db.Corporates.Where(f => f.CorporateCustId == account.CorporateCustId)
                        .Select(g => g.CompanyName)).FirstOrDefault(),
                    BaseType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.BaseType)).FirstOrDefault(),
                    AccountStatus = account.AccountStatus,
                    PurposeId = account.PurposeId

                });
            }
            return Ok(accountAll);
        }

        // GET: api/Accounts/Corporates/5
        [HttpGet("accounts/Corporates/{id}")]
        public IActionResult GetAccountCorporate(int id)
        {
            IEnumerable<Account> scAccounts = _db.Accounts.Where(a => a.CorporateCustId == id);
            List<AccountAllModel> accountAll = new List<AccountAllModel>();
            foreach (Account account in scAccounts)
            {
                accountAll.Add(new AccountAllModel
                {
                    AccountId = account.AccountId,
                    AvailableBalance = account.AvailableBalance,
                    TotalBalance = account.TotalBalance,
                    AccountNumber = account.AccountNumber,
                    AccountName = account.IndividualCustId != null ? (_db.Individuals.Where(e => e.IndividualCustId == account.IndividualCustId)
                        .Select(f => f.FirstName + " " + f.LastName)).FirstOrDefault() : (_db.Corporates.Where(f => f.CorporateCustId == account.CorporateCustId)
                        .Select(g => g.CompanyName)).FirstOrDefault(),
                    BaseType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.BaseType)).FirstOrDefault(),
                    AccountStatus = account.AccountStatus,
                    PurposeId = account.PurposeId

                });
            }
            return Ok(accountAll);
        }

        // GET: api/Accounts/5
        [HttpGet("accounts/JointCustomers/{id}")]
        public IActionResult GetAccountJoint(int id)
        {
            IEnumerable<Account> scAccounts = _db.Accounts.Where(a => a.JointId == id);
            List<AccountAllModel> accountAll = new List<AccountAllModel>();
            foreach (Account account in scAccounts)
            {
                accountAll.Add(new AccountAllModel
                {
                    AccountId = account.AccountId,
                    AvailableBalance = account.AvailableBalance,
                    TotalBalance = account.TotalBalance,
                    AccountNumber = account.AccountNumber,
                    AccountName = account.IndividualCustId != null ? (_db.Individuals.Where(e => e.IndividualCustId == account.IndividualCustId)
                        .Select(f => f.FirstName + " " + f.LastName)).FirstOrDefault() : (_db.Corporates.Where(f => f.CorporateCustId == account.CorporateCustId)
                        .Select(g => g.CompanyName)).FirstOrDefault(),
                    BaseType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.BaseType)).FirstOrDefault(),
                    AccountStatus = account.AccountStatus,
                    PurposeId = account.PurposeId

                });
            }
            return Ok(accountAll);
        }

        // PUT: api/Accounts/5
        [HttpPut("accounts/{id}")]
        public async Task<IActionResult> PutAccount(int id, [FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != account.AccountId)
            {
                return BadRequest();
            }
            account.CreatedUserId = (_db.Accounts.Where(a => a.AccountId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            account.CreatedDate = (_db.Accounts.Where(a => a.AccountId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(account).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        //[HttpPut("accounts/status/{id}")]
        //public async Task<IActionResult> PutStatus(int id, [FromBody] Account _account)
        //{

        //    Account account = await _db.Accounts.FindAsync(id);
        //    if (account == null)
        //    {
        //        return BadRequest($"Account with ID - {id} does not exist.");
        //    }
        //    try
        //    {
        //        account.AccountStatus = _account.AccountStatus;
        //        account.UpdatedUser = _account.UpdatedUser;
        //        account.UpdatedDate = _account.UpdatedDate;
        //        _db.Entry(account).State = EntityState.Modified;
        //        _db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!AccountExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPost("accounts")]
        public async Task<IActionResult> PostAccount([FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var accountNumber = GenerateAccountNumber(account);
            account.AccountNumber = accountNumber;
            account.AccountStatus = "Inactive";
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();
            return Ok(new {Status = "OK", Message = "Successfully Added", Output = "Account has been added"});
        }

        [NonAction]
        public string GenerateAccountNumber(Account account)
        {
            var branchcode = (_db.BranchDetails.Where(a => a.BranchId == account.BranchId).Select(a => a.BranchCode)).FirstOrDefault();
            var accounttypecode =
                (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(a => a.AccountTypeCode))
                .FirstOrDefault();
            string customerNumber = null;
            if (account.IndividualCustId != null)
            {
                customerNumber =
                (_db.Individuals.Where(a => a.IndividualCustId == account.IndividualCustId)
                    .Select(a => a.CustomerNumber)).FirstOrDefault();
            }
            else if (account.CorporateCustId != null)
            {
                customerNumber =
                (_db.Corporates.Where(a => a.CorporateCustId == account.CorporateCustId)
                    .Select(a => a.CorporateNumber)).FirstOrDefault();
            }
            else if (account.JointCustomer != null)
            {
                customerNumber =
                (_db.JointCustomers.Where(a => a.JointId == account.JointId)
                    .Select(a => a.JointNumber)).FirstOrDefault();
            }
            var accountNumber=_tranService.GetAccountNumber(branchcode, accounttypecode, customerNumber);
            return accountNumber;
        }

        // DELETE: api/Accounts/5
        [HttpDelete("accounts/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            Account account = await _db.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync();

            return Ok(account);
        }

        [NonAction]
        private bool AccountExists(int id)
        {
            return _db.Accounts.Count(e => e.AccountId == id) > 0;
        }
    }

    public class AccountAllModel
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BaseType { get; set; }
        public decimal? TotalBalance { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string PurposeId { get; set; }
        public string AccountStatus { get; set; }
        public string AccountType { get; set; }
    }

}