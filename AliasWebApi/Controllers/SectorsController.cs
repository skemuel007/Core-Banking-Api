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
    public class SectorsController : Controller
    {
        private readonly AppDbContext _db;

        public SectorsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Sectors
        [HttpGet]
        public IQueryable<Sector> GetSectors()
        {
            return _db.Sectors;
        }

        // GET: api/Sectors/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSector(int id)
        {
            Sector sector = await _db.Sectors.FindAsync(id);
            if (sector == null)
            {
                return NotFound();
            }

            return Ok(sector);
        }

        // PUT: api/Sectors/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutSector(int id,[FromBody] Sector sector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sector.SectorId)
            {
                return BadRequest();
            }

            sector.CreatedUserId = (_db.Sectors.Where(a => a.SectorId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            sector.CreatedDate = (_db.Sectors.Where(a => a.SectorId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(sector).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SectorExists(id))
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

        // POST: api/Sectors
        [HttpPost]
        public async Task<IActionResult> PostSector([FromBody] Sector sector)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Sectors.Add(sector);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Sector has been added" });
        }

        // DELETE: api/Sectors/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSector(int id)
        {
            Sector sector = await _db.Sectors.FindAsync(id);
            if (sector == null)
            {
                return NotFound();
            }

            _db.Sectors.Remove(sector);
            await _db.SaveChangesAsync();

            return Ok(sector);
        }

        [NonAction]
        private bool SectorExists(int id)
        {
            return _db.Sectors.Count(e => e.SectorId == id) > 0;
        }
    }
}