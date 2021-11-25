using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using AliasWebApiCore.Models.Identity;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/count")]
    public class CountController : Controller
    {
        private readonly AppDbContext _db;
        
        public CountController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{tablename}")]
        public int Get(string tablename)
        {
            var count = _db.Database.ExecuteSqlCommand($"select count (*) from {tablename}");//.SqlQuery<int>($"select count (*) from {tablename}");
            return count;
        }

        //[HttpPost("Receivemobilemoney")]
        //public async Task<IActionResult> ReceiveMobileMoney([FromBody] SendModel model)
        //{
        //    var httpclient = new HttpClient();
        //    var json = JsonConvert.SerializeObject(model);
        //    HttpContent httpContent = new StringContent(json);
        //    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    var byteArray = Encoding.ASCII.GetBytes("tqtzvsys:kvyonlnb");
        //    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        //    var response = await httpclient.PostAsync("https://api.hubtel.com/v1/merchantaccount/merchants/HM2305170047/receive/mobilemoney", httpContent);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        HttpContent httpcontent = response.Content;
        //        Task<string> content = httpcontent.ReadAsStringAsync();
        //        string result = content.Result;
        //        var returnjson = JsonConvert.DeserializeObject<ReceiveModel>(result);
        //        return Ok(returnjson);
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}

        //public class SendModel
        //{
        //    public string CustomerName { get; set; }
        //    public string CustomerMsisdn { get; set; }
        //    public string CustomerEmail { get; set; }
        //    public string Channel { get; set; }
        //    public decimal Amount { get; set; }
        //    public string PrimaryCallbackUrl { get; set; }
        //    public string SecondaryCallbackUrl { get; set; }
        //    public string Description { get; set; }
        //    public string ClientReference { get; set; }
        //    public string Token { get; set; }
        //    public bool FeesOnCustomer { get; set; }
        //}

        //public class ReceiveModel
        //{
        //    public string ResponseCode { get; set; }
        //    public Data Data { get; set; }
        //}

        //public class Data
        //{
        //    public decimal AmountAfterCharges { get; set; }
        //    public decimal AmountCharged { get; set; }
        //    public string TransactionId { get; set; }
        //    public string ClientReference { get; set; }
        //    public string Description { get; set; }
        //    public string ExternalTransactionId { get; set; }
        //    public decimal Amount { get; set; }
        //    public decimal Charges { get; set; }
        //    public string Meta { get; set; }
        //}

    }
}