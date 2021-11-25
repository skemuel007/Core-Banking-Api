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
    [Route("api/[controller]")]
    [Authorize]
    public class UserGroupsController : Controller
    {
        private readonly AppDbContext _db;

        public UserGroupsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/UserGroups
        [HttpGet("")]
        public IQueryable<UserGroup> GetUserGroups()
        {
            return _db.UserGroups;
        }

        // GET: api/UserGroups/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserGroup(int id)
        {
            UserGroup userGroup = await _db.UserGroups.FindAsync(id);
            if (userGroup == null)
            {
                return NotFound();
            }

            return Ok(userGroup);
        }

        // PUT: api/UserGroups/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutUserGroup(int id,[FromBody] UserGroup userGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userGroup.CreatedUserId)
            {
                return BadRequest();
            }

            _db.Entry(userGroup).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserGroupExists(id))
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

        // POST: api/UserGroups
        [HttpPost("")]
        public async Task<IActionResult> PostUserGroup([FromBody] UserGroup userGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.UserGroups.Add(userGroup);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserGroupExists(userGroup.CreatedUserId))
                {
                    return BadRequest();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "UserGroup has been added" });
        }

        // DELETE: api/UserGroups/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUserGroup(int id)
        {
            UserGroup userGroup = await _db.UserGroups.FindAsync(id);
            if (userGroup == null)
            {
                return NotFound();
            }

            _db.UserGroups.Remove(userGroup);
            await _db.SaveChangesAsync();

            return Ok(userGroup);
        }

        [NonAction]
        private bool UserGroupExists(int? id)
        {
            return _db.UserGroups.Count(e => e.CreatedUserId == id) > 0;
        }
    }
}