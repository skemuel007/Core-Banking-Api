using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Corporates")]
    public class CorporatesController : Controller
    {
        private readonly AppDbContext _db;

        public CorporatesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Corporates
        [HttpGet]
        public IQueryable<Corporate> GetCorporates()
        {
            return _db.Corporates;
        }

        // GET: api/Corporates/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCorporate(int id)
        {
            Corporate corporate = await _db.Corporates.FindAsync(id);
            if (corporate == null)
            {
                return NotFound();
            }

            return Ok(corporate);
        }

        // PUT: api/Corporates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCorporate(int id,[FromBody] Corporate corporate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != corporate.CorporateCustId)
            {
                return BadRequest();
            }

            corporate.CreatedUserId = (_db.Corporates.Where(a => a.CorporateCustId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            corporate.CreatedDate = (_db.Corporates.Where(a => a.CorporateCustId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(corporate).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CorporateExists(id))
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

        // POST: api/Corporates
        [HttpPost]
        public async Task<IActionResult> PostCorporate([FromBody] Corporate corporate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Corporates.Add(corporate);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Corporate has been added" });
        }

        // DELETE: api/Corporates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCorporate(int id)
        {
            Corporate corporate = await _db.Corporates.FindAsync(id);
            if (corporate == null)
            {
                return NotFound();
            }

            _db.Corporates.Remove(corporate);
            await _db.SaveChangesAsync();

            return Ok(corporate);
        }

        [NonAction]
        private bool CorporateExists(int id)
        {
            return _db.Corporates.Count(e => e.CorporateCustId == id) > 0;
        }
    }
}