using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountTypesController : Controller
    {
        private readonly AppDbContext _db;

        public AccountTypesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/AccountTypes
        [HttpGet]
        public IQueryable<AccountTypes> GetAccountTypes()
        {
            return _db.AccountTypes;
        }

        [HttpGet("{basetype}")]
        public IQueryable<AccountTypes> GetAccountTypes(string basetype)
        {
            return _db.AccountTypes.Where(a=>a.BaseType==basetype);
        }

        // GET: api/AccountTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountTypes(int id)
        {
            AccountTypes accountTypes = await _db.AccountTypes.FindAsync(id);
            if (accountTypes == null)
            {
                return NotFound();
            }

            return Ok(accountTypes);
        }

        // PUT: api/AccountTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccountTypes(int id,[FromBody] AccountTypes accountTypes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != accountTypes.AccountTypeId)
            {
                return BadRequest();
            }

            accountTypes.CreatedUserId = (_db.AccountTypes.Where(a => a.AccountTypeId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            accountTypes.CreatedDate = (_db.AccountTypes.Where(a => a.AccountTypeId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(accountTypes).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountTypesExists(id))
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

        // POST: api/AccountTypes
        [HttpPost]
        public async Task<IActionResult> PostAccountTypes([FromBody] AccountTypes accountTypes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (accountTypes.GeneralLedgerCodeId != null &&
                _db.GeneralLedgerCodes.Any(a => a.GeneralLedgerCodeId == accountTypes.GeneralLedgerCodeId))
            {
                
            }

            _db.AccountTypes.Add(accountTypes);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Account Type has been added" });
        }

        // DELETE: api/AccountTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountTypes(int id)
        {
            AccountTypes accountTypes = await _db.AccountTypes.FindAsync(id);
            if (accountTypes == null)
            {
                return NotFound();
            }

            _db.AccountTypes.Remove(accountTypes);
            await _db.SaveChangesAsync();

            return Ok(accountTypes);
        }

        [NonAction]
        private bool AccountTypesExists(int id)
        {
            return _db.AccountTypes.Count(e => e.AccountTypeId == id) > 0;
        }
    }
}