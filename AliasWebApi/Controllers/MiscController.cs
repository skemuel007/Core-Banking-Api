using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AliasWebApiCore.Models;
using AliasWebApiCore.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AliasWebApi.Models;
using AliasWebApiCore.Models.Identity;
using AutoMapper;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Bcpg;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class MiscController : Controller
    {
        private readonly AppDbContext _db;
        //private readonly IMapper _mapper;
        private readonly ITransactionService _transService;

        public MiscController(AppDbContext db, /*IMapper mapper,*/ ITransactionService transService)
        {
            _db = db;
            //_mapper = mapper;
            _transService = transService;
        }

        [HttpPost("import")]
        public IActionResult Import([FromBody] ImportModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            switch (model.Type.ToLower())
            {
                case "individual":
                    _db.Individuals.AddRange(model.Individuals);
                    break;
                case "corporate":
                    _db.Corporates.AddRange(model.Corporates);
                    break;
                case "joint":
                    break;
            }
            _db.SaveChanges();
            return Ok("Import Successfull");
        }

     

      

        [NonAction]
        public void ImportJoint()
        {

        }

        public class ImportModel
        {
           public string Type { get; set; }
           public List<Individual> Individuals { get; set; }
           public List<Corporate> Corporates { get; set; }
        }

        //[HttpPost("GenerateTrialBal")]
        //public IActionResult Gen([FromBody] GenModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var trialbalance = new List<TrialModel>();
        //    var nominalCodes = _db.Transactions.Select(a => a.GeneralLedgerCodeId).ToList();
        //    for (int i = 0; i < nominalCodes.Count; i++)
        //    {
        //        var debit = (_db.Transactions.Where(a =>
        //            a.GeneralLedgerCodeId == nominalCodes[i] && a.SessionDate > model.StartDate &&
        //            a.SessionDate < model.EndDate).Select(a => a.Debit)).Sum();
        //        var credit = (_db.Transactions.Where(a =>
        //            a.GeneralLedgerCodeId == nominalCodes[i] && a.SessionDate > model.StartDate &&
        //            a.SessionDate < model.EndDate).Select(a => a.Debit)).Sum();
        //        var openingbal = ((_db.Transactions.Where(a =>
        //                                 a.GeneralLedgerCodeId == nominalCodes[i] && a.SessionDate < model.StartDate)
        //                             .Select(a => a.Debit)).Sum())
        //                         - ((_db.Transactions.Where(a =>
        //                                 a.GeneralLedgerCodeId == nominalCodes[i] && a.SessionDate < model.StartDate)
        //                             .Select(a => a.Credit)).Sum());
        //        trialbalance[i].OpeningBalance = openingbal == 0 ? openingbal : (openingbal < 0 ? openingbal * (-1) : openingbal);
        //        trialbalance[i].Description = ((_db.GeneralLedgerCodes.Where(a => a.GeneralLedgerCodeId == nominalCodes[i]).Select(a => a.Description)).FirstOrDefault());
        //        trialbalance[i].Debits = (debit);
        //        trialbalance[i].Credits = (credit);
        //        trialbalance[i].ClosingBalance = ((debit > credit) ? (debit - credit) + " Dr" : (credit - debit) + " Cr");
        //    }
        //    return Ok(trialbalance);
        //}

        //public class GenModel
        //{
        //    public DateTime StartDate { get; set; }
        //    public DateTime EndDate { get; set; }
        //}

        //public class TrialModel
        //{
        //    public string Description { get; set; }
        //    public decimal OpeningBalance { get; set; }
        //    public decimal Debits { get; set; }
        //    public decimal Credits { get; set; }
        //    public string ClosingBalance { get; set; }
        //}

        [HttpPost("SmsGeneralBroadCast")]
        public async Task<IActionResult> Sms([FromBody] SmsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var phonenumbers = new List<string>();
            var individualNumbers=(_db.Individuals.Select(a => a.Mobile));
            foreach (var number in individualNumbers)
            {
                phonenumbers.Add(number+',');
            }
            var corporateNumbers = (_db.Corporates.Select(a => a.CompanyPhone));
            foreach (var number in corporateNumbers)
            {
                phonenumbers.Add(number+',');
            }
            if (phonenumbers.Count == 0)
            {
                return Ok("No phone numbers found");
            }
            SmsConfig smsconfig = _db.SmsConfig.FirstOrDefault(a => a.Status.ToLower().Contains("active"));
            if (smsconfig == null)
            {
                return Content("No Sms Configuration Found");
            }
            else
            {
                try
                {
                    var httpClient = new HttpClient();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(smsconfig.HostUrl).Append("?&username=").Append(smsconfig.UserName)
                        .Append("&password=").Append(smsconfig.Password).Append("&source=").Append(smsconfig.Sender)
                        .Append("&destination=").Append(phonenumbers).Append("&message=").Append(model.Message);
                    var json = await httpClient.GetStringAsync(sb.ToString());
                    var smsresponse = JsonConvert.DeserializeObject<SmsResponse>(json);
                    return Ok(new { ResponseCode = smsresponse.Code, Message = smsresponse.Message });
                }
                catch (Exception e)
                {
                    return Content(e.Message);
                }
            }
        }

        public class SmsModel
        {
            public string Message { get; set; }
        }

        ////[HttpPost("SmsGeneralBroadcast")]
        ////public async Task<bool> SmsGeneralBroadcast([FromBody] GeneralSmsModel model)
        ////{
        ////    List<Account> accounts = _db.Accounts.ToList();
        ////    List<FixedDeposit> fdaccounts = _db.FixedDeposit.ToList();
        ////    //List<string> phonenumbers = new List<string>();
        ////    string fullname = null;
        ////    foreach (Account account in accounts)
        ////    {
        ////        if (account.IndividualCustId != null)
        ////        {
        ////            var individual =
        ////                _db.Individuals.FirstOrDefault(a => a.IndividualCustId == account.IndividualCustId);
        ////            //phonenumbers.Add(individual?.KPhone);
        ////            //fullname = individual?.FirstName + " " + individual?.LastName;
        ////            SendSms(model.Message, individual?.KPhone);
        ////        }
        ////        else if (account.CorporateCustId != null)
        ////        {
        ////            var corporate = _db.Corporates.FirstOrDefault(a => a.CorporateCustId == account.CorporateCustId);
        ////            //phonenumbers.Add(corporate?.CompanyPhone);
        ////            fullname = corporate.CompanyName;
        ////        }
        ////        else
        ////        {
        ////            //var individual = (_db.JointCustomersKeys.Where(a => a.JointId == account.JointId)
        ////            //    .Select(b => b.Individual);
        ////            //phonenumbers.Add((_db.JointCustomersKeys.Where(a => a.JointId == account.JointId).Select(b => b.Individual)
        ////            //    .Select(a => a.KPhone)).FirstOrDefault());
        ////            //fullname =
        ////        }
        ////    }
        ////}

        ////[NonAction]
        ////private async void SendSms( string message, string phonenumber)
        ////{
        ////    SmsConfig smsconfig = _db.SmsConfig.FirstOrDefault(a => a.Status.ToLower().Contains("active"));
        ////    if (smsconfig == null)
        ////    {
        ////        return;
        ////    }
        ////    else
        ////    {
        ////        var httpClient = new HttpClient();
        ////        StringBuilder sb = new StringBuilder();
        ////        try
        ////        {
        ////            sb.Append(smsconfig.HostUrl).Append("?&username=").Append(smsconfig.UserName)
        ////                .Append("&password=").Append(smsconfig.Password).Append("&source=").Append(smsconfig.Sender);
        ////            sb.Append("&destination=").Append(phonenumber).Append("&message=").Append(message);
        ////            var json = await httpClient.GetStringAsync(sb.ToString());
        ////            var smsresponse = JsonConvert.DeserializeObject<SmsResponse>(json);
        ////            tosend.ResponseCode = smsresponse.Code;
        ////            tosend.Status = true;
        ////            tosend.SentDate = DateTime.Now;
        ////            _db.Entry(tosend).State = EntityState.Modified;
        ////        }
        ////    }
        ////}


        //[HttpPost("SavingsInterest")]
        //public async Task<IActionResult> SavingsInterest(SavingsInterestModel model)
        //{
        //    List<AccountFormat> accountFormat = new List<AccountFormat>();
        //    foreach (int interesttype in model.InterestTypes)
        //    {
        //        accountFormat.Add(new AccountFormat {AccountTypeId = interesttype});
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
        //    return Ok();
        //}


        //[HttpPost("Cot")]
        //public async Task<bool> Cot(SavingsInterestModel model)
        //{
        //    List<AccountFormat> accountFormat = new List<AccountFormat>();
        //    foreach (int interesttype in model.InterestTypes)
        //    {
        //        accountFormat.Add(new AccountFormat {AccountTypeId = interesttype});
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

        [HttpGet("GlBalance/{gl}")]
        public IActionResult GlBalance(int gl)
        {
            if (!_db.GeneralLedgerCodes.Any(a => a.GeneralLedgerCodeId == gl))
            {
                return BadRequest("Gl does not exist");
            }
            var debit = (_db.Transactions.Where(a => a.GeneralLedgerCodeId == gl).Select(a => a.Debit)).Sum();
            var credit = (_db.Transactions.Where(a => a.GeneralLedgerCodeId == gl).Select(a => a.Credit)).Sum();
            var bal = debit - credit > 0 ? (debit - credit) + " Dr" : (debit - credit) + " Cr";
            return Ok(bal);
        }

        [HttpGet("CahBookSummary")]
        public IActionResult CashBookSummary()
        {
            List<TellerSummary> tellerSummary = new List<TellerSummary>();
            for (int i = 0; i < _db.Transactions.Select(a => a.GeneralLedgerCodeId).Count(); i++)
            {
                tellerSummary = (from a in _db.GeneralLedgerCodes
                    join b in _db.Transactions on a.GeneralLedgerCodeId
                        equals b.GeneralLedgerCodeId
                    join c in _db.Teller on
                        b.GeneralLedgerCodeId equals c.GeneralLedgerCodeId
                    select new TellerSummary
                    {
                        Account = a.SubCode,
                        Description = a.Description,
                        Balance = (b.Debit - b.Credit) > 0 ? (b.Debit - b.Credit) + " Dr" : (b.Debit - b.Credit) + " Cr"
                    }).ToList();
            }
            return Ok(tellerSummary);
        }

        public class TellerSummary
        {
            public string Account { get; set; }
            public string Description { get; set; }
            public string Balance { get; set; }
        }

        [HttpPost("Approval")]
        public async Task<IActionResult> Approve([FromBody] ApprovalModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (model.AccountId != null)
            {
                string result=await ApproveAccount(model.AccountId,model.UserId,model.Status);
                if (result == null)
                {
                   
                    return Ok();
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                string result=await ApproveTransaction(model);
                if (result == null)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result);
                }
            }
        }

        [NonAction]
        private async Task<string> ApproveTransaction(ApprovalModel model)
        {
            TransCodeItems approvalItem =
               _db.TransCodeItems.FirstOrDefault(a =>
                   a.TransCode == model.TransCode && a.Status.ToLower().Contains("pending"));
            if (approvalItem == null)
            {
                return "Item does not exist or has already been approved";
            }
            Account account = await _db.Accounts.FindAsync(approvalItem.AccountId);
            if (account == null)
            {
                return "Account not found";
            }
            int approvalid = (_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(a => a.ApprovalRuleId)).FirstOrDefault();
            ApprovalRules approvalRule =
                _db.ApprovalRules.Find(approvalid);
            if (approvalRule == null)
            {
                return "No approval rule associated with account";
            }
            bool canApprove = false;
            foreach (string userid in approvalRule.ApproversUserIds.Split(',').ToList())
            {
                if (model.UserId.ToString() == userid)
                {
                    if (approvalItem.Debit >= approvalRule.MinimumAmount ||
                        approvalItem.Debit <= approvalRule.MaximumAmount)
                    {
                        canApprove = true;
                    }
                    else if (approvalItem.Credit >= approvalRule.MinimumAmount ||
                             approvalItem.Credit <= approvalRule.MaximumAmount)
                    {
                        canApprove = true;
                    }
                }
            }
            if (canApprove)
            {
                if (approvalItem.TransSource.ToLower().Contains("deposit"))
                {
                    if (approvalItem.TransSource.ToLower().Contains("cheque"))
                    {
                        Cheques cheque = _db.Cheques.FirstOrDefault(a => a.ChequeNumber == model.ChequeNumber);
                        cheque.Status = "Approved";
                        _db.Entry(cheque).State = EntityState.Modified;
                        account.AvailableBalance += Convert.ToInt32(approvalItem.Credit);
                        account.TotalBalance += Convert.ToInt32(approvalItem.Credit);
                        approvalItem.Status = "Updated";
                        _db.Entry(account).State = EntityState.Modified;
                        _db.Entry(approvalItem).State = EntityState.Modified;
                        _db.SaveChanges();
                        _transService.ConfigureAccountTransaction("deposit", account, null, approvalItem.TransCode,approvalItem);
                    }
                    else
                    {
                        account.AvailableBalance += Convert.ToInt32(approvalItem.Credit);
                        account.TotalBalance += Convert.ToInt32(approvalItem.Credit);
                        approvalItem.Status = "Updated";
                        _db.Entry(account).State = EntityState.Modified;
                        _db.Entry(approvalItem).State = EntityState.Modified;
                        _db.SaveChanges();
                        _transService.ConfigureAccountTransaction("deposit", account,  null, approvalItem.TransCode, approvalItem);
                    }
                }
                else if (approvalItem.TransSource.ToLower().Contains("withdrawal"))
                {
                    if (approvalItem.TransSource.ToLower().Contains("cheque"))
                    {
                        Cheques cheque = _db.Cheques.FirstOrDefault(a => a.ChequeNumber == model.ChequeNumber);
                        cheque.Status = "Approved";
                        account.AvailableBalance -= Convert.ToInt32(approvalItem.Debit);
                        account.TotalBalance -= Convert.ToInt32(approvalItem.Debit);
                        approvalItem.Status = "Updated";
                        _db.Entry(account).State = EntityState.Modified;
                        _db.Entry(approvalItem).State = EntityState.Modified;
                        _transService.ConfigureAccountTransaction("deposit", account, null, approvalItem.TransCode, approvalItem);
                        _db.Entry(cheque).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    account.AvailableBalance -= Convert.ToInt32(approvalItem.Debit);
                    account.TotalBalance -= Convert.ToInt32(approvalItem.Debit);
                    approvalItem.Status = "Updated";
                    _db.Entry(account).State = EntityState.Modified;
                    _db.Entry(approvalItem).State = EntityState.Modified;
                    _transService.ConfigureAccountTransaction("deposit", account,  null, approvalItem.TransCode, approvalItem);
                    _db.SaveChanges();
                }
                //else if(approvalItem.TransSource.ToLower().Contains("topup"))
            }
            else
            {
                return "User is not authorized to approve this transaction";
            }
            //_db.SaveChanges();
            return null;
        }

        [NonAction]
        private async Task<string> ApproveAccount(int? accountId,int userid,string status)
        {
            Account account = await _db.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return "Account not found";
            }
            ApprovalRules approvalRule =
                _db.ApprovalRules.Find((_db.AccountTypes.Where(a => a.AccountTypeId == account.AccountTypeId).Select(b => b.ApprovalRuleId)).FirstOrDefault());
            if (approvalRule == null)
            {
                return "No Approval Rule associated with the account";
            }
            bool canApprove = false;
            foreach (string userId in approvalRule.ApproversUserIds.Split(',').ToList())
            {
                if (userid.ToString() == userId)
                {
                    canApprove = true;
                }
            }
            if (canApprove)
            {
                account.AccountStatus = status;
                _db.Entry(account).State = EntityState.Modified;
                _db.SaveChanges();
            }
            return null;
        }

        [HttpPost("TellerTransfer")]
        public async Task<IActionResult> TellerTransfer([FromBody] TellerActivityModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_db.GeneralLedgerCodes.Any(a =>
                a.GeneralLedgerCodeId == model.FromGl || a.GeneralLedgerCodeId == model.ToGl))
            {
                return BadRequest("One or both general ledger codes do not exist");
            }
            await SaveToTransactionAsync(model,"teller transfer");
            return Ok("Teller transfer successfull");
        }

        [HttpPost("TellerVoucher")]
        public async Task<IActionResult> TellerVoucher([FromBody] TellerActivityModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_db.GeneralLedgerCodes.Any(a =>
                a.GeneralLedgerCodeId == model.FromGl || a.GeneralLedgerCodeId == model.ToGl))
            {
                return BadRequest("One or both general ledger codes do not exist");
            }
            await SaveToTransactionAsync(model,"Teller Voucher");
            return Ok("Teller Voucher successfull");
        }

        [HttpPost]
        [Route("Sweep")]
        public IActionResult Sweep(Sweep sweep)
        {
            if (ModelState.IsValid)
            {
                if (sweep.Amount >= sweep.MinimumBalance)
                {
                    _db.Sweeps.Add(sweep);
                    _db.SaveChanges();
                }
                else
                {
                    return BadRequest("Sweep Amount cannot be less than the minimum balance");
                }
            }
            return BadRequest(ModelState);
        }

        [NonAction]
        private async Task<bool> SaveToTransactionAsync(TellerActivityModel model,string operation)
        {
            try
            {
                Transaction trans1 = new Transaction
                {
                    GeneralLedgerCodeId = model.FromGl,
                    LedgerType = "Nominal",
                    SessionDate =
                    (_db.SessionManager.Where(a => a.Status.ToLower() == "active")
                        .Select(b => b.SessionDate)).FirstOrDefault(),
                    Debit = model.Amount,
                    Credit = 0,
                    TransSource = operation
                };
                Transaction trans2 = new Transaction
                {
                    GeneralLedgerCodeId = model.ToGl,
                    LedgerType = "Nominal",
                    SessionDate =
                    (_db.SessionManager.Where(a => a.Status.ToLower() == "active")
                        .Select(b => b.SessionDate)).FirstOrDefault(),
                    Debit = 0,
                    Credit = model.Amount,
                    TransSource = operation
                };
                _db.Transactions.Add(trans1);
                _db.Transactions.Add(trans2);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpPost("NominalEnquiry")]
        public IActionResult NominalEnquiry([FromBody] TellerActivityModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_db.GeneralLedgerCodes.Any(a =>
                a.GeneralLedgerCodeId == model.NominalGl))
            {
                return BadRequest("Nominal Code does not exist");
            }
            var nominalTransactions = _db.Transactions.Where(a => a.LedgerType.ToLower() == "nominal" && a.GeneralLedgerCodeId == model.NominalGl);
            var debit =
                (nominalTransactions.Where(a => a.SessionDate < model.StartDate).Select(a => a.Debit)).Sum();
            var credit =
                (nominalTransactions.Where(a => a.SessionDate < model.StartDate).Select(a => a.Credit)).Sum();
            var openingBalance = debit > credit ? (debit - credit) + " Dr" : (credit - debit) + " Cr";
            var filteredbydate =
                 nominalTransactions.Where(a => a.SessionDate <= model.EndDate && a.SessionDate >= model.StartDate).ToList();
            var nominalenquiry=new NominalEnquiryModel();
            for (int i=0;i<filteredbydate.Count;i++)
            {
                nominalenquiry.Details.Add(new Details
                {
                    Date = Convert.ToDateTime(filteredbydate[i].SessionDate),
                    Debit = filteredbydate[i].Debit,
                    Credit = filteredbydate[i].Debit,
                    Reference = filteredbydate[i].Reference,
                    TransactionCode = filteredbydate[i].TransCode,
                    Balance = nominalenquiry.Details.Count==0? CalculateBal(openingBalance, filteredbydate[i].Debit, filteredbydate[i].Credit):
                          CalculateBal(nominalenquiry.Details[i-1].Balance, filteredbydate[i].Debit, filteredbydate[i].Credit)
                });
            }
            return Ok(nominalenquiry);
        }

        [HttpGet("GetTransCodes")]
        public IActionResult GetTransactionCodes()
        {
            return Ok(_db.TransCodeItems.Select(a=>a.TransCode).Distinct());
        }

        //[HttpGet("GetTransCode")]
        //public IActionResult GetTransactionCodes()
        //{
        //    return Ok(_db.TransCodeItems.Select(a => a.TransCode).Distinct());
        //}

        [HttpGet("GetBatch/{transcode}")]
        public IActionResult GetBatch(string transcode)
        {
            return Ok(_db.TransCodeItems.GroupBy(a => a.TransCode == transcode));
        }

        [NonAction]
        private string CalculateBal(string balance, decimal debit, decimal credit)
        {
            string bal = null;
            var balanceIsDebit = balance.ToLower().Contains("dr");
            if (debit > 0)
            {
                switch (balanceIsDebit)
                {
                    case true:
                        bal= (Convert.ToDecimal(balance) + debit) + " Dr";
                        break;
                    case false:
                        bal = debit > Convert.ToDecimal(balance)
                            ? (debit - Convert.ToDecimal(balance)) + " Dr"
                            : (Convert.ToDecimal(balance) - debit) + " Cr";
                        break;
                }
            }
            else if (credit > 0)
            {
                switch (balanceIsDebit)
                {
                    case true:
                        bal = credit > Convert.ToDecimal(balance)
                            ? (credit - Convert.ToDecimal(balance)) + " Cr"
                            : (Convert.ToDecimal(balance) - credit) + " Dr";
                        break;
                    case false:
                        bal = (Convert.ToDecimal(balance) + credit) + " Cr";
                        break;
                }
            }
            return bal;
        }

        public class NominalEnquiryModel
        {
            public string OpeningBalance { get; set; }
            public List<Details> Details { get; set; }
        }
        public class Details
        {
            public DateTime Date { get; set; }
            public string TransactionCode { get; set; }
            public string Reference { get; set; }
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public string Balance { get; set; }
        }

        public class TellerActivityModel
        {
            public int FromGl { get; set; }
            public int ToGl { get; set; }
            public decimal Amount { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int NominalGl { get; set; }
        }

        //[NonAction]
        //private void MovetoTransaction(string type,TransCodeItems transcode,string chequeNumber=null,Account account=null)
        //{
        //    switch (type)
        //    {
        //        case "deposit":
        //            break;
        //        case "withdrawal":
        //            break;
        //    }
        //    var trans1 = new Transaction
        //    {
        //        Credit = Convert.ToDecimal(transcode.Credit),
        //        Debit = Convert.ToDecimal(transcode.Debit),
        //        AccountId = Convert.ToInt32(transcode.AccountId),
        //        LedgerType = transcode.LedgerType,
        //        GeneralLedgerCodeId = transcode.GeneralLegderCodeId,
        //        TransSource = transcode.TransSource,
        //        TransCode = transcode.TransCode,
        //        BranchId = transcode.BranchId,
        //        ChequeNumber = chequeNumber
        //    };
        //    var trans2 = new Transaction
        //    {
        //        Credit = Convert.ToDecimal(transcode.Credit),
        //        Debit = Convert.ToDecimal(transcode.Debit),
        //        AccountId = Convert.ToInt32(transcode.AccountId),
        //        LedgerType = transcode.LedgerType,
        //        GeneralLedgerCodeId = transcode.GeneralLegderCodeId,
        //        TransSource = transcode.TransSource,
        //        TransCode = transcode.TransCode,
        //        BranchId = transcode.BranchId,
        //        ChequeNumber = chequeNumber
        //    };
        //    _db.Transactions.Add(trans1);
        //    _db.SaveChanges();
        //    _db.Transactions.Add(trans2);
        //    _db.SaveChanges();
        //}
    }

    public class GeneralSmsModel
    {
        public string Message { get; set; }
    }
    public class ApprovalModel
    {
        public string TransCode { get; set; }
        public int UserId { get; set; }
        public int? AccountId { get; set; }
        public string ChequeNumber { get; set; }
        public string Status { get; set; }
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

    //public class DomainProfile : Profile
    //{
    //    public DomainProfile()
    //    {
    //        CreateMap<TransCodeItems, Transaction>();
    //    }
    //}

}