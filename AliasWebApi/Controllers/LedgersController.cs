using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LedgersController : Controller
    {
        private readonly AppDbContext _db;

        public LedgersController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Ledgers
        [HttpGet]
        public IQueryable<Ledgers> GetLedgers()
        {
            return _db.Ledgers;
        }

        // GET: api/Ledgers/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLedgers(int id)
        {
            Ledgers ledgers = await _db.Ledgers.FindAsync(id);
            if (ledgers == null)
            {
                return NotFound();
            }

            return Ok(ledgers);
        }

        // PUT: api/Ledgers/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutLedgers(int id,[FromBody] Ledgers ledgers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ledgers.LedgerId)
            {
                return BadRequest();
            }

            ledgers.CreatedUserId = (_db.Ledgers.Where(a => a.LedgerId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            ledgers.CreatedDate = (_db.Ledgers.Where(a => a.LedgerId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(ledgers).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LedgersExists(id))
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

        // POST: api/Ledgers
        [HttpPost]
        public async Task<IActionResult> PostLedgers([FromBody] Ledgers ledgers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Ledgers.Add(ledgers);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Ledger has been added" });
        }

        // DELETE: api/Ledgers/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLedgers(int id)
        {
            Ledgers ledgers = await _db.Ledgers.FindAsync(id);
            if (ledgers == null)
            {
                return NotFound();
            }

            _db.Ledgers.Remove(ledgers);
            await _db.SaveChangesAsync();

            return Ok(ledgers);
        }

        [NonAction]
        private bool LedgersExists(int id)
        {
            return _db.Ledgers.Count(e => e.LedgerId == id) > 0;
        }
    }
}