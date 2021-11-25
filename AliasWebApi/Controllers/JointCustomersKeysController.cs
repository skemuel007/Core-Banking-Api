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
    public class JointCustomersKeysController : Controller
    {
        private readonly AppDbContext _db;

        public JointCustomersKeysController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/JointCustomersKeys
        [HttpGet]
        public IQueryable<JointCustomersKeys> GetJointCustomersKeys()
        {
            return _db.JointCustomersKeys
                .Include(j => j.JointCustomer)
            .Include(c => c.Individual);
        }

        // GET: api/JointCustomersKeys/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetJointCustomersKeys(int id)
        {
            var jointCustomerkeys = _db.JointCustomersKeys.Include(k => k.JointCustomer).Include(a => a.Individual);
            var filtered = await jointCustomerkeys.FirstOrDefaultAsync(a => a.JointId == id);
            return Ok(filtered);
        }

        // PUT: api/JointCustomersKeys/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutJointCustomersKeys(int id,[FromBody] JointCustomersKeys jointCustomersKeys)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != jointCustomersKeys.JointCustKeysId)
            {
                return BadRequest();
            }

            jointCustomersKeys.CreatedUserId = (_db.JointCustomersKeys.Where(a => a.JointCustKeysId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            jointCustomersKeys.CreatedDate = (_db.JointCustomersKeys.Where(a => a.JointCustKeysId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(jointCustomersKeys).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JointCustomersKeysExists(id))
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

        // POST: api/JointCustomersKeys
        [HttpPost]
        public async Task<IActionResult> PostJointCustomersKeys([FromBody] JointCustomersKeys jointCustomersKeys)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.JointCustomersKeys.Add(jointCustomersKeys);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Joint Customer Key has been added" });
        }

        // DELETE: api/JointCustomersKeys/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteJointCustomersKeys(int id)
        {
            JointCustomersKeys jointCustomersKeys = await _db.JointCustomersKeys.FindAsync(id);
            if (jointCustomersKeys == null)
            {
                return NotFound();
            }

            _db.JointCustomersKeys.Remove(jointCustomersKeys);
            await _db.SaveChangesAsync();

            return Ok(jointCustomersKeys);
        }

        [NonAction]
        private bool JointCustomersKeysExists(int id)
        {
            return _db.JointCustomersKeys.Count(e => e.JointCustKeysId == id) > 0;
        }
    }
}