using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ASLApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BanktiersController : Controller
    {
        private readonly AppDbContext _db;

        public BanktiersController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Banktiers
        [HttpGet]
        public IQueryable<Banktiers> GetBanktiers()
        {
            return _db.Banktiers;
        }

        // GET: api/Banktiers/cot
        [HttpGet("type")]
        public IQueryable<Banktiers> GetBanktiersbytype(string type)
        {
            return _db.Banktiers.Where(a=>a.Type.ToLower().Contains(type));
        }

        // GET: api/Banktiers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBanktiers(int id)
        {
            Banktiers banktiers = await _db.Banktiers.FindAsync(id);
            if (banktiers == null)
            {
                return NotFound();
            }

            return Ok(banktiers);
        }

        // PUT: api/Banktiers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBanktiers(int id,[FromBody] Banktiers banktiers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != banktiers.BanktiersId)
            {
                return BadRequest();
            }

            banktiers.CreatedUserId = (_db.Banktiers.Where(a => a.BanktiersId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            banktiers.CreatedDate = (_db.Banktiers.Where(a => a.BanktiersId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(banktiers).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BanktiersExists(id))
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

        // POST: api/Banktiers
        [HttpPost]
        public async Task<IActionResult> PostBanktiers([FromBody] Banktiers banktiers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Banktiers.Add(banktiers);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Bank Tier has been added" });
        }

        // DELETE: api/Banktiers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanktiers(int id)
        {
            Banktiers banktiers = await _db.Banktiers.FindAsync(id);
            if (banktiers == null)
            {
                return NotFound();
            }

            _db.Banktiers.Remove(banktiers);
            await _db.SaveChangesAsync();

            return Ok(banktiers);
        }

        [NonAction]
        private bool BanktiersExists(int id)
        {
            return _db.Banktiers.Count(e => e.BanktiersId == id) > 0;
        }
    }
}