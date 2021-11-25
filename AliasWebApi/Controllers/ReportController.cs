using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using AliasWebApiCore.Signal_R;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Schedule")]
    public class ReportController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IDataProtector _protector;
        //private readonly IHubContext<Alerts> _hubContext;

        public ReportController(AppDbContext db, IDataProtectionProvider provider/*,IHubContext<Alerts> hubContext*/)
        {
            _db = db;
            _protector = provider.CreateProtector("Contoso.MyClass.v1");
            //_hubContext = hubContext;
        }

        [HttpPost]
        public object GenerateSchedule([FromBody] ScheduleModel model)
        {
            if (ModelState.IsValid)
            {
                FixedDeposit account = _db.FixedDeposit.FirstOrDefault(a => a.Account.AccountNumber == model.AccountNumber);
                if (account == null)
                {
                    return BadRequest($"Account number {model.AccountNumber} does not exist.");
                }
                int? accountid = account?.FixedDepositId;
                List<TransactionFormat> transformat = (from a in _db.Transactions
                    where a.AccountId == accountid
                    select new TransactionFormat
                    {
                        SessionDates = a.SessionDate,
                        Credits = a.Credit
                    }).ToList();
                double period = Convert.ToDouble(account?.FixedDepositPeriod);
                decimal? principal = account.FixedDepositPrincipal;
                DateTime? startDate = account.Account.OpenedDate;
                DateTime? enddate = DateTime.Parse(startDate.ToString()).AddDays(period - 1);
                List<ReportModel> report = new List<ReportModel>();
                int no = (int) period + 1;
                decimal?[] balance = new decimal?[no];
                balance[0] = principal;
                decimal? fixedprincipal = account.FixedDepositPrincipal;
                for (int i = 0; i < period; i++)
                {
                    decimal? interestamount =
                        ((Convert.ToDecimal(account.FixedDepositInterestRate) / 100) * principal) / 365;
                    balance[i + 1] = Convert.ToDecimal(String.Format("{0:0.##}", interestamount));

                    foreach (TransactionFormat tformat in transformat)
                    {
                        if (DateTime.Parse(startDate.ToString()).AddDays(i) == tformat.SessionDates)
                        {
                            principal += tformat.Credits;
                            interestamount = ((Convert.ToDecimal(account.FixedDepositInterestRate) / 100) * principal) /
                                             365;
                            balance[i + 1] = Convert.ToDecimal(String.Format("{0:0.##}", interestamount));
                        }

                    }
                    report.Add(new ReportModel
                    {
                        Date = (DateTime.Parse(startDate.ToString()).AddDays(i)).ToString(@"yyyy-MM-dd "),
                        Principal = string.Format("{0:0.##}", principal),
                        Interest = String.Format("{0:0.##}", interestamount),
                        Balance = string.Format("{0:0.##}", balance.Sum())
                    });
                }
                //return report;
                var dataList = report;
                //column Header name
                var columnsHeader = new List<string>
                {
                    "No",
                    "Date",
                    "Principal (GHS)",
                    "Interest",
                    "Balance (GHS)"
                };
                var filecontent = ExportPDF(dataList, columnsHeader, "Schedule_Report");
                return File(filecontent, "application/pdf", "Schedule_Report.pdf");
            }
            return null;
        }

        [NonAction]
        private byte[] ExportPDF(List<ReportModel> dataList, List<string> columnsHeader, string heading)
        {

            var document = new Document();
            var outputMS = new MemoryStream();
            var writer = PdfWriter.GetInstance(document, outputMS);
            document.Open();
            var font5 = FontFactory.GetFont(FontFactory.HELVETICA, 11);

            document.Add(new Phrase(Environment.NewLine));

            //var count = typeof(UserListVM).GetProperties().Count();
            var count = columnsHeader.Count;
            var table = new PdfPTable(count);
            float[] widths = new float[] {2f, 4f, 5f, 4f, 4f};

            table.SetWidths(widths);

            table.WidthPercentage = 100;
            var cell = new PdfPCell(new Phrase(heading));
            cell.Colspan = count;

            for (int i = 0; i < count; i++)
            {
                var headerCell = new PdfPCell(new Phrase(columnsHeader[i], font5));
                headerCell.BackgroundColor = BaseColor.Gray;
                table.AddCell(headerCell);
            }

            var sn = 1;
            foreach (var item in dataList)
            {
                table.AddCell(new Phrase(sn.ToString(), font5));
                table.AddCell(new Phrase(item.Date, font5));
                table.AddCell(new Phrase(item.Principal, font5));
                table.AddCell(new Phrase(item.Interest, font5));
                table.AddCell(new Phrase(item.Balance, font5));

                sn++;
            }

            document.Add(table);
            document.Close();
            var result = outputMS.ToArray();

            return result;
        }

        //[HttpPost("api/protection/{data}")]
        //public IActionResult pup(string data)
        //{
        //    string pdata = _protector.Protect(data);
        //    string unpdata = _protector.Unprotect(pdata);
        //    //_hubContext.Clients.All.InvokeAsync("send", "Hello from the server");
        //    return Ok(new {OriginalText = pdata, ProtectedText = unpdata});
        //}

        //[AllowAnonymous]
        //[HttpPost("api/message")]
        //public IActionResult Start([FromBody] ChatModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    _hubContext.Clients.All.InvokeAsync("Send",model.Username);
        //    return Ok();
        //}
    }

    public class ChatModel
    {
        public string Message { get; set; }
        public string Username { get; set; }
    }

    public class TransactionFormat
        {
            public DateTime? SessionDates { get; set; }
            public decimal? Credits { get; set; }
        }

        public class ScheduleModel
        {
            public string AccountNumber { get; set; }

        }

        public class ReportModel
        {

            public string Date { get; set; }
            public string Principal { get; set; }
            public string Interest { get; set; }
            public string Balance { get; set; }
        }

    }
