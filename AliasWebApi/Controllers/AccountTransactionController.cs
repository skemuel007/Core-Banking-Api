using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using AliasWebApiCore.Services;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountTransactionController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ITransactionService _transService;

        public AccountTransactionController(AppDbContext db, ITransactionService transService)
        {
            _db = db;
            _transService = transService;
        }

        [HttpPost("CashDeposit")]
        public async Task<IActionResult> CashDeposit([FromBody] TransactionAccept transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                Account account = await _db.Accounts.FindAsync(transaction.transaction.AccountId);
                if (account == null)
                {
                    return NotFound($"Account with id {transaction.transaction.AccountId} does not exist");
                }
                try
                {
                    Teller teller = _db.Teller.Find(transaction.TellerId);
                    if (teller == null)
                    {
                        return BadRequest("No teller id provided");
                    }
                    transaction.transaction.GeneralLedgerCodeId = teller.GeneralLedgerCodeId;
                    string transcode = _transService.GetTransCode("transaction");
                if (transaction.transaction.Credit > teller?.DepositLimit)
                    {
                        _transService.SendForApproval("Pending",transaction,transcode);
                        return BadRequest("Cash Deposit Limit has been reached for teller. Request has been sent for approval ");
                    }
                    account.TotalBalance += transaction.transaction.Credit;
                    account.AvailableBalance += transaction.transaction.Credit;
                    _db.Entry(account).State = EntityState.Modified;
                    _db.SaveChanges();
                    

                _transService.ConfigureAccountTransaction("deposit", account, transaction.transaction,transcode);

                _transService.SendForApproval("Updated",transaction,transcode);

                return Ok(new ResponseFormat { Status = "Successfully Deposited", TotalBalance = account.TotalBalance, AvailableBalance = account.AvailableBalance });
                }
                catch (DbUpdateConcurrencyException e)
                {
                    return Content(e.Message);
                }
            
        }

        [HttpPost("CashWithDrawal")]
        public async Task<IActionResult> CashWithDrawal([FromBody] TransactionAccept transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Account account = await _db.Accounts.FindAsync(transaction.transaction.AccountId);
            if (account == null)
            {
                return NotFound($"Account with id {transaction.transaction.AccountId} does not exist");
            }
            try
            {
                if (account.AvailableBalance < transaction.transaction.Debit)
                {
                    return BadRequest("Insufficient funds.Cannot perform transaction");
                }
                Teller teller = _db.Teller.Find(transaction.TellerId);

                if (teller == null)
                {
                    return BadRequest("No teller id provided");
                }
                transaction.transaction.GeneralLedgerCodeId = teller.GeneralLedgerCodeId;
                string transcode = _transService.GetTransCode("transaction");
                if (transaction.transaction.Debit > teller?.WithdrawalLimit)
                {
                    _transService.SendForApproval("Pending", transaction,transcode);
                    return BadRequest("Cash Withdrawal Limit has been reached for teller.");
                }

                account.TotalBalance -= transaction.transaction.Debit;
                account.AvailableBalance -= transaction.transaction.Debit;
                _db.Entry(account).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                
                _transService.ConfigureAccountTransaction("withdrawal",account, transaction.transaction,transcode);
                _transService.SendForApproval("Updated", transaction,transcode);

                return Ok(new ResponseFormat { Status = "Successfully Withdrawn", TotalBalance = account.TotalBalance, AvailableBalance = account.AvailableBalance });

            }
            catch (DbUpdateConcurrencyException e)
            {
                StringContent sc = new StringContent(e.ToString());
                return NoContent();
            }
        }

        [HttpPost("ChequeDeposit")]
        public async Task<IActionResult> ChequeDeposit([FromBody] TransactionAccept transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Account account = await _db.Accounts.FindAsync(transaction.transaction.AccountId);
            if (account == null)
            {
                return NotFound($"Account with id {transaction.transaction.AccountId} does not exist");
            }
            //cheque table entry
            Cheques cheque = transaction.Cheque;
            cheque.Status = "pending";
            _db.Cheques.Add(cheque);
            try
            {
                Teller teller = _db.Teller.Find(transaction.TellerId);
                if (teller == null)
                {
                    return BadRequest("No teller id provided");
                }
                transaction.transaction.GeneralLedgerCodeId = teller.GeneralLedgerCodeId;
                string transcode = _transService.GetTransCode("transaction");
                if (transaction.transaction.Credit > teller?.DepositLimit)
                {

                    _transService.SendForApproval("Pending", transaction,transcode);
                    return BadRequest("Cheque Deposit Limit has been reached for teller.");
                }
                switch (transaction.ClearingType.ToLower())
                {
                    case "internal_clearing":
                        account.TotalBalance += transaction.transaction.Credit;
                        account.AvailableBalance += transaction.transaction.Credit;
                        break;
                    case "external_clearing":
                        account.TotalBalance += transaction.transaction.Credit;
                        break;
                    case "company":
                        break;
                }

                _db.Entry(account).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                
                _transService.ConfigureAccountTransaction("deposit",account, transaction.transaction, transcode);
                _transService.SendForApproval("Updated", transaction,transcode);

                return Ok(new ResponseFormat { Status = "Successfully Deposited", TotalBalance = account.TotalBalance, AvailableBalance = account.AvailableBalance });
            }
            catch (DbUpdateConcurrencyException e)
            {
                return Ok(e.Message);
            }
        }

        [HttpPost("ChequeWithDrawal")]
        public async Task<IActionResult> ChequeWithDrawal([FromBody] TransactionAccept transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Account account = await _db.Accounts.FindAsync(transaction.transaction.AccountId);
            if (account == null)
            {
                return NotFound($"Account with id {transaction.transaction.AccountId} does not exist");
            }
            //cheque table entry
            Cheques cheque = transaction.Cheque;
            cheque.Status = "pending";
            _db.Cheques.Add(cheque);
            try
            {
                if (account.TotalBalance < transaction.transaction.Credit || account.AvailableBalance < transaction.transaction.Credit)
                {
                    return BadRequest("Insufficient funds.Cannot perform transaction");
                }
                Teller teller = _db.Teller.Find(transaction.TellerId);
                if (teller == null)
                {
                    return BadRequest("No teller id provided");
                }
                transaction.transaction.GeneralLedgerCodeId = teller.GeneralLedgerCodeId;
                string transcode = _transService.GetTransCode("transaction");
                if (transaction.transaction.Debit > teller?.WithdrawalLimit)
                {
                    _transService.SendForApproval("Pending", transaction,transcode);
                    return BadRequest("Cheque Withdrawal Limit has been reached for teller.");
                }

                switch (transaction.ClearingType.ToLower())
                {
                    case "internal_clearing":
                        account.TotalBalance -= transaction.transaction.Debit;
                        account.AvailableBalance -= transaction.transaction.Debit;
                        break;
                    case "external_clearing":
                        account.TotalBalance -= transaction.transaction.Debit;
                        break;
                    case "company":
                        break;
                }
                _db.Entry(account).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                //cheque table entry
                _transService.ConfigureAccountTransaction("withdrawal", account, transaction.transaction, transcode);
                _transService.SendForApproval("Updated", transaction,transcode);

                return Ok(new ResponseFormat { Status = "Successfully Withdrawn", TotalBalance = account.TotalBalance, AvailableBalance = account.AvailableBalance });
            }
            catch (DbUpdateConcurrencyException e)
            {
                return Content(e.Message);
            }
        }

        [HttpPost("Lien")]
        public async Task<IActionResult> AddLien([FromBody]Liens liens)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Account account = await _db.Accounts.FindAsync(liens.AccountId);
            if (account == null)
            {
                return NotFound($"Account with id {liens.AccountId} does not exist");
            }
            try
            {
                if (liens.Amount > account.AvailableBalance)
                {
                    return BadRequest($"Lien cannot be greater than Available Balance");
                }
                account.AvailableBalance -= liens.Amount;
                _db.Entry(account).State = EntityState.Modified;
                _db.Liens.Add(liens);

                //var transcode = _transService.GetTransCode("lien");
                //var trans = new Transaction
                //{
                //    TransCode = transcode,
                //    AccountId = Convert.ToInt32(liens.AccountId),
                //    TransSource = "Lien",
                //    GeneralLedgerCodeId = (_db.AccountTypes.Where(a=>a.AccountTypeId==account.AccountTypeId).Select(a=>a.GeneralLedgerCodeId)).FirstOrDefault(),
                //    BranchId = liens.BranchId,
                    

                //};
                //_db.Transactions.Add(trans);
                _db.SaveChanges();
                return Ok($"Lien of GHS {liens.Amount} has been applied to Account Number - {account.AccountNumber}");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        [HttpPut]
        [Route("Cheques/{id}")]
        public async Task<IActionResult> Clear(int id, [FromBody] Cheques Cheques)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Account account = _db.Accounts.Find(Cheques.AccountId);
            Cheques cheques = _db.Cheques.Find(id);
            if (account == null)
            {
                return NotFound($"Account with id - {Cheques.AccountId} could not be found");
            }
            if (cheques == null)
            {
                return NotFound($"Cheque with id - {id} could not be found");
            }
            switch (Cheques.Status.ToLower())
            {
                case "cleared":
                    account.AvailableBalance += Cheques.ChequeAmount;
                    _db.Entry(account).State = EntityState.Modified;
                    cheques.Status = Cheques.Status;
                    cheques.ClearedDate = Cheques.ClearedDate;
                    cheques.ClearedUserId = Cheques.ClearedUserId;
                    cheques.Note = Cheques.Note;
                    _db.Entry(cheques).State = EntityState.Modified;
                    break;
                case "declined":
                    account.TotalBalance -= Cheques.ChequeAmount;
                    _db.Entry(account).State = EntityState.Modified;
                    cheques.Status = Cheques.Status;
                    cheques.ClearedDate = Cheques.ClearedDate;
                    cheques.ClearedUserId = Cheques.ClearedUserId;
                    cheques.Note = Cheques.Note;
                    _db.Entry(cheques).State = EntityState.Modified;
                    break;
            }
            await _db.SaveChangesAsync();
            return Ok(" Cheque processed");
        }

        [HttpGet]
        public IActionResult CashierSummary()
        {
            List<CashierSummary> cashiersummary = new List<CashierSummary>();
            for (int i = 0; i < _db.GeneralLedgerCodes.Count(); i++)
            {
                cashiersummary = (from b in _db.Teller
                                  join c in _db.Users on b.CreatedUserId equals c.UserId
                                  join d
                                      in _db.BranchDetails on b.BranchId equals d.BranchId
                                  join h in _db.GeneralLedgerCodes
                                      on b.GeneralLedgerCodeId equals h.GeneralLedgerCodeId
                                  select new CashierSummary
                                  {
                                      CashierName = h.SubCode,
                                      UserName = c.UserName,
                                      Pin = b.Pin,
                                      DepositLimit = b.DepositLimit,
                                      WithDrawalLimit = b.WithdrawalLimit,
                                      BranchName = d.BranchName,
                                      Transactions = (_db.Transactions.Where(a => a.CreatedUserId == c.UserId).Where(y => y.GeneralLedgerCodeId == h.GeneralLedgerCodeId).Where(z => z.LedgerType.ToLower() == "nominal")).Count(),
                                      Balance = ((from g in _db.Transactions where g.CreatedUserId == c.UserId && g.GeneralLedgerCodeId == h.GeneralLedgerCodeId && g.LedgerType.ToLower() == "nominal" select (Decimal?)g.Debit).Sum() ?? 0)
                                                - ((from g in _db.Transactions where g.CreatedUserId == c.UserId && g.GeneralLedgerCodeId == h.GeneralLedgerCodeId && g.LedgerType.ToLower() == "nominal" select (Decimal?)g.Credit).Sum() ?? 0),
                                      TellerId = b.TellerID
                                  }
                   ).ToList();
            }
            return Ok(cashiersummary);
        }

      


    }
}
