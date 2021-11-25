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
    public class GroupsController : Controller
    {
        private readonly AppDbContext _db;

        public GroupsController(AppDbContext db)
        {
            _db = db;
        }
        
        // GET: api/Groups
        [HttpGet]
        public IQueryable<Group> GetGroups()
        {
            return _db.Groups
                .Include(u => u.UserGroups);
        }

        // GET: api/Groups/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGroup(int id)
        {
            Group group = await _db.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        // PUT: api/Groups/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutGroup(int id,[FromBody] Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != group.GroupId)
            {
                return BadRequest();
            }

            _db.Entry(group).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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

        // POST: api/Groups
        [HttpPost]
        public async Task<IActionResult> PostGroup([FromBody] Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Groups.Add(group);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Group has been added" });
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            Group group = await _db.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            _db.Groups.Remove(group);
            await _db.SaveChangesAsync();

            return Ok(group);
        }

        [NonAction]
        private bool GroupExists(int id)
        {
            return _db.Groups.Count(e => e.GroupId == id) > 0;
        }
    }
}