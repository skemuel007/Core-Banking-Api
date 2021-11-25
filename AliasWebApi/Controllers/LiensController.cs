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
    public class LiensController : Controller
    {
        private readonly AppDbContext _db;

        public LiensController(AppDbContext db)
        {
            _db = db;
        }


        // GET: api/Liens
        [HttpGet]
        public IQueryable<Liens> GetLiens()
        {
            return _db.Liens;
        }

        // GET: api/Liens/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLien(int id)
        {
            Liens lien = await _db.Liens.FindAsync(id);
            if (lien == null)
            {
                return NotFound();
            }

            return Ok(lien);
        }



        // PUT: api/Liens/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutLien(int id,[FromBody] Liens liens)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != liens.LienId)
            {
                return BadRequest();
            }

            liens.CreatedUserId = (_db.Liens.Where(a => a.LienId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            liens.CreatedDate = (_db.Liens.Where(a => a.LienId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(liens).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LienExists(id))
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

        // POST: api/Liens
        [HttpPost]
        public async Task<IActionResult> PostAccount([FromBody] Liens liens)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Liens.Add(liens);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Lien has been added" });
        }

        // DELETE: api/Liens/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLien(int id)
        {
            Liens liens = await _db.Liens.FindAsync(id);
            if (liens == null)
            {
                return NotFound();
            }

            _db.Liens.Remove(liens);
            await _db.SaveChangesAsync();

            return Ok(liens);
        }

        [NonAction]
        private bool LienExists(int id)
        {
            return _db.Liens.Count(e => e.LienId == id) > 0;
        }
    }
}
