using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AliasWebApiCore.Services;
using AliasWebApiCore.Models.Identity;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api")]

    public class BatchPostingController : Controller
    {

        private readonly AppDbContext _db;
        private readonly ITransactionService _transService;

        public BatchPostingController(AppDbContext db, ITransactionService transService)
        {
            _transService = transService;
            _db = db;
        }

        [HttpPost("BatchPosting")]
        public IActionResult BatchPosting([FromBody] BatchModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (FormattedTransCode transitem in model.TransCodeItems)
            {
                Account account = _db.Accounts.FirstOrDefault(a => a.AccountNumber == transitem.AccountNumber);
                var glcode =
                    (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(a => a.GeneralLedgerCodeId))
                    .FirstOrDefault();
                //if (!(_db.GeneralLedgerCodes.Any(a => a.GeneralLedgerCodeId == glcode)))
                //{
                //    return BadRequest($"GeneralLedgerCode does not exist for Gl id- {transitem.GeneralLegderCodeId}");
                //}
                
                if (transitem.Debit > account.AvailableBalance )
                {
                    return BadRequest("Available balance on account is less than the withdrawal amount");
                }
                if (transitem.Debit > 0 && transitem.Debit < account.AvailableBalance)
                {
                    account.TotalBalance -= Convert.ToInt32(transitem.Debit);
                    account.AvailableBalance -= Convert.ToInt32(transitem.Debit);
                    _db.SaveChanges();
                }
                //else if (transitem.Credit > 0)
                //{
                //    account.TotalBalance += transitem.Credit;
                //    account.AvailableBalance += transitem.Credit;
                //    _db.SaveChanges();
                //}
                switch (model.Operation.ToLower())
                {
                    case "save":
                        if (_db.TransCodeItems.Any(a => a.TransCode == transitem.TransCode))
                        {
                            TransCodeItems transcodeitem =
                                _db.TransCodeItems.FirstOrDefault(a => a.TransCode == transitem.TransCode);
                            if (transcodeitem != null)
                            {
                                _db.Remove(transcodeitem);
                                _db.SaveChanges();
                            }
                            var transcode = _transService.GetTransCode("transaction");
                                TransCodeItems transcodeitems = new TransCodeItems
                                {
                                    Debit = transitem.Debit,
                                    Credit = transitem.Credit,
                                    AccountId =account.AccountId,
                                    Reference = transitem.Reference,
                                    Status = "Saved",
                                    BranchId = Convert.ToInt32(transitem.BranchId),
                                    SessionDate = (_db.SessionManager.Where(a => a.Status.ToLower().Contains("active")).Select(a => a.SessionDate)).FirstOrDefault(),
                                    TransCode = transcode/*_transService.GetTransCode("transaction")*/,
                                    GeneralLedgerCodeId = Convert.ToInt32(glcode),
                                    TransSource = "Batch Posting",
                                    CreatedUserId = Convert.ToInt32(transitem.CreatedUserId),
                                    LedgerType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(a => a.LedgerType)).FirstOrDefault(),
                                    CreatedDate = DateTime.Now
                                };
                                _db.TransCodeItems.Add(transcodeitems);
                                _db.SaveChanges();
                                return Ok("Batch Posting Successfull");
                        }
                        break;
                    case "post":
                        if (_db.TransCodeItems.Any(a => a.TransCode == transitem.TransCode))
                        {
                            TransCodeItems transcodeitem =
                                _db.TransCodeItems.FirstOrDefault(a => a.TransCode == transitem.TransCode);
                            if (transcodeitem != null)
                            {
                                _db.Remove(transcodeitem);
                                _db.SaveChanges();
                            }
                            var transcode = _transService.GetTransCode("transaction");
                            TransCodeItems transcodeitems = new TransCodeItems
                            {
                                Debit = transitem.Debit,
                                Credit = transitem.Credit,
                                AccountId = account.AccountId,
                                Reference = transitem.Reference,
                                Status = "Pending",
                                BranchId = Convert.ToInt32(transitem.BranchId),
                                SessionDate = (_db.SessionManager.Where(a => a.Status.ToLower().Contains("active")).Select(a => a.SessionDate)).FirstOrDefault(),
                                TransCode = transcode/*_transService.GetTransCode("transaction")*/,
                                GeneralLedgerCodeId = glcode,
                                TransSource = "Batch Posting",
                                CreatedUserId = Convert.ToInt32(transitem.CreatedUserId),
                                LedgerType = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(a => a.LedgerType)).FirstOrDefault(),
                                CreatedDate = DateTime.Now
                            };
                            _db.TransCodeItems.Add(transcodeitems);
                            _db.SaveChanges();
                            return Ok("Batch Posting Successfull");
                        }
                        break;
                }
            }
            return Ok("Batch Posting Successfull");
        }

        [HttpPost("NominalPosting")]
        public IActionResult NominalPosting([FromBody] BatchModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            foreach (FormattedTransCode transitem in model.TransCodeItems)
            {
                //if (!(_db.GeneralLedgerCodes.Any(a => a.GeneralLedgerCodeId == transitem.GeneralLegderCodeId)))
                //{
                //    return BadRequest($"GeneralLedgerCode does not exist for Gl id- {transitem.GeneralLegderCodeId}");
                //}
                switch (model.Operation.ToLower())
                {
                    case "save":
                        if (_db.TransCodeItems.Any(a => a.TransCode == transitem.TransCode))
                        {
                            TransCodeItems transcodeitem =
                                _db.TransCodeItems.FirstOrDefault(a => a.TransCode == transitem.TransCode);
                            if (transcodeitem != null)
                            {
                                _db.Remove(transcodeitem);
                                _db.SaveChanges();
                            }

                            TransCodeItems transcodeitems = new TransCodeItems
                            {
                                Debit = transitem.Debit,
                                Credit = transitem.Credit,
                                AccountId = null,
                                Reference = transitem.Reference,
                                Status = "Saved",
                                BranchId = Convert.ToInt32(transitem.BranchId),
                                SessionDate = (_db.SessionManager.Where(a => a.Status.ToLower().Contains("active")).Select(a => a.SessionDate)).FirstOrDefault(),
                                TransCode = _transService.GetTransCode("transaction"),
                                //GeneralLegderCodeId = transitem.GeneralLegderCodeId,
                                TransSource = "Nominal Posting",
                                CreatedUserId = Convert.ToInt32(transitem.CreatedUserId),
                                CreatedDate = DateTime.Now
                            };
                            _db.TransCodeItems.Add(transcodeitems);
                            _db.SaveChanges();
                            return Ok("Batch Posting Successfull");
                        }
                        break;
                    case "post":
                        if (_db.TransCodeItems.Any(a => a.TransCode == transitem.TransCode))
                        {
                            TransCodeItems transcodeitem =
                                _db.TransCodeItems.FirstOrDefault(a => a.TransCode == transitem.TransCode);
                            if (transcodeitem != null)
                            {
                                _db.Remove(transcodeitem);
                                _db.SaveChanges();
                            }

                            TransCodeItems transcodeitems = new TransCodeItems
                            {
                                Debit = transitem.Debit,
                                Credit = transitem.Credit,
                                AccountId = null,
                                Reference = transitem.Reference,
                                Status = "Pending",
                                BranchId = Convert.ToInt32(transitem.BranchId),
                                SessionDate = (_db.SessionManager.Where(a=>a.Status.ToLower().Contains("active")).Select(a=>a.SessionDate)).FirstOrDefault(),
                                TransCode = _transService.GetTransCode("transaction"),
                                //GeneralLegderCodeId = transitem.GeneralLegderCodeId,
                                TransSource = "Nominal Posting",
                                CreatedUserId = Convert.ToInt32(transitem.CreatedUserId),
                                CreatedDate = DateTime.Now
                            };
                            _db.TransCodeItems.Add(transcodeitems);
                            _db.SaveChanges();
                            return Ok("Batch Posting Successfull");
                        }
                        break;
                }
            }
            return Ok("Nominal Posting Successfull");
        }

        [HttpGet("")]
        public IActionResult Duplicate()
        {
            return Ok(_transService.GetTransCode("transaction"));
        }

    }

    public class BatchModel
    {
        public IEnumerable<FormattedTransCode> TransCodeItems { get; set; }
        public string Operation { get; set; }
    }

    public class FormattedTransCode
    {
        public string LedgerType { get; set; }
        public int? LoanId { get; set; }
        public string TransCode { get; set; }
        public string Reference { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public string TransSource { get; set; }
        public string Status { get; set; }
        public DateTime? SessionDate { get; set; }
        public int? BranchId { get; set; }
        public string AccountNumber { get; set; }
        public int? CreatedUserId { get; set; }
    }

}