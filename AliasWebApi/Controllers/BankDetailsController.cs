using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BankDetailsController : Controller
    {
        private readonly AppDbContext _db;

        public BankDetailsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/BankDetails
        [HttpGet]
        public IQueryable<BankDetails> GetBankDetails()
        {
            return _db.BankDetails;
        }

        // GET: api/BankDetails/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankDetails(int id)
        {
            BankDetails bankDetails = await _db.BankDetails.FindAsync(id);
            if (bankDetails == null)
            {
                return NotFound();
            }

            return Ok(bankDetails);
        }

        // PUT: api/BankDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankDetails(int id,[FromBody] BankDetails bankDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bankDetails.BankDetailsId)
            {
                return BadRequest();
            }

            bankDetails.CreatedUserId = (_db.BankDetails.Where(a => a.BankDetailsId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            bankDetails.CreatedDate = (_db.BankDetails.Where(a => a.BankDetailsId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(bankDetails).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankDetailsExists(id))
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

        // POST: api/BankDetails
        [HttpPost]
        public async Task<IActionResult> PostBankDetails([FromBody] BankDetails bankDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.BankDetails.Add(bankDetails);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Bank Detail has been added" });
        }

        // DELETE: api/BankDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankDetails(int id)
        {
            BankDetails bankDetails = await _db.BankDetails.FindAsync(id);
            if (bankDetails == null)
            {
                return NotFound();
            }

            _db.BankDetails.Remove(bankDetails);
            await _db.SaveChangesAsync();

            return Ok(bankDetails);
        }

        [NonAction]
        private bool BankDetailsExists(int id)
        {
            return _db.BankDetails.Count(e => e.BankDetailsId == id) > 0;
        }
    }
}