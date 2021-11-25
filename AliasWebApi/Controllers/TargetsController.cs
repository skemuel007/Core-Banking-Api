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
    public class TargetsController : Controller
    {
        private readonly AppDbContext _db;

        public TargetsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Targets
        [HttpGet]
        public IQueryable<Target> GetTargets()
        {
            return _db.Targets;
        }

        // GET: api/Targets/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTarget(int id)
        {
            Target target = await _db.Targets.FindAsync(id);
            if (target == null)
            {
                return NotFound();
            }

            return Ok(target);
        }

        // PUT: api/Targets/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutTarget(int id,[FromBody] Target target)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != target.TargetId)
            {
                return BadRequest();
            }

            target.CreatedUserId = (_db.Targets.Where(a => a.TargetId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            target.CreatedDate = (_db.Targets.Where(a => a.TargetId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(target).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TargetExists(id))
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

        // POST: api/Targets
        [HttpPost]
        public async Task<IActionResult> PostTarget([FromBody] Target target)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Targets.Add(target);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Target has been added" });
        }

        // DELETE: api/Targets/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTarget(int id)
        {
            Target target = await _db.Targets.FindAsync(id);
            if (target == null)
            {
                return NotFound();
            }

            _db.Targets.Remove(target);
            await _db.SaveChangesAsync();

            return Ok(target);
        }

        [NonAction]
        private bool TargetExists(int id)
        {
            return _db.Targets.Count(e => e.TargetId == id) > 0;
        }
    }
}