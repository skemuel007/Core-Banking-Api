using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/AccountPopupMsg")]
    public class AccountPopupMsgController : Controller
    {
        private readonly AppDbContext _db;

        public AccountPopupMsgController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{accountId}")]
        public IActionResult GetAccountPopUpMsg(int accountId)
        {
            return Ok(_db.AccountPopupMsg.FirstOrDefault(a => a.AccountId == accountId && a.Status));
        }

        [HttpPost("")]
        public IActionResult SetAccountPopUpMsg([FromBody] AccountPopupMsg model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_db.AccountPopupMsg.Any(a => a.AccountId == model.AccountId))
            {
                model.Status = true;
                _db.AccountPopupMsg.Add(model);
            }
            else if (_db.AccountPopupMsg.Any(a => a.AccountId == model.AccountId && a.Status==false))
            {
                model.Status = true;
                _db.AccountPopupMsg.Add(model);
            }
            else if (_db.AccountPopupMsg.Any(a => a.AccountId == model.AccountId && a.Status))
            {
                var existingAccountPopup =
                    _db.AccountPopupMsg.FirstOrDefault(a => a.AccountId == model.AccountId && a.Status);
                if (existingAccountPopup != null)
                {
                    existingAccountPopup.Status = false;
                    existingAccountPopup.CreatedUserId = existingAccountPopup.CreatedUserId;
                    existingAccountPopup.CreatedDate = existingAccountPopup.CreatedDate;
                    _db.Entry(existingAccountPopup).State = EntityState.Modified;
                    _db.AccountPopupMsg.Add(model);
                }
            }
            _db.SaveChanges();
            return Ok();
        }
    }
}