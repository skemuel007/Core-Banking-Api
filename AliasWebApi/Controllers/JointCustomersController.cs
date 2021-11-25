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
    public class JointCustomersController : Controller
    {
        private readonly AppDbContext _db;

        public JointCustomersController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/JointCustomers
        [HttpGet]
        public IQueryable<JointCustomer> GetJointCustomers()
        {
            return
                _db.JointCustomers.Include(k => k.JointCustomersKeys).ThenInclude(b => b.Individual).Include(a => a.Accounts);
        }

        // GET: api/JointCustomers/5
        [HttpGet("{id:int}")]
        public async Task<object> GetJointCustomer(int id)
        {
            var jointCustomer = _db.JointCustomers.Include(k => k.JointCustomersKeys).ThenInclude(b => b.Individual).Include(a => a.Accounts);
            var filtered = await jointCustomer.FirstOrDefaultAsync(a => a.JointId == id);
            return Ok(filtered);
        }

        // PUT: api/JointCustomers/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutJointCustomer(int id,[FromBody] JointCustomer jointCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != jointCustomer.JointId)
            {
                return BadRequest();
            }

            jointCustomer.CreatedUserId = (_db.JointCustomers.Where(a => a.JointId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            jointCustomer.CreatedDate = (_db.JointCustomers.Where(a => a.JointId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(jointCustomer).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JointCustomerExists(id))
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

        // POST: api/JointCustomers
        [HttpPost]
        public async Task<IActionResult> PostJointCustomer([FromBody] JointCustomer jointCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.JointCustomers.Add(jointCustomer);
            await _db.SaveChangesAsync();
            
            return Ok(new { Status = "OK", Message = "Successfully Added", Output = $"{_db.JointCustomers.Last().JointId}" });
        }

        // DELETE: api/JointCustomers/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteJointCustomer(int id)
        {
            JointCustomer jointCustomer = await _db.JointCustomers.FindAsync(id);
            if (jointCustomer == null)
            {
                return NotFound();
            }

            _db.JointCustomers.Remove(jointCustomer);
            await _db.SaveChangesAsync();

            return Ok(jointCustomer);
        }

        [NonAction]
        private bool JointCustomerExists(int id)
        {
            return _db.JointCustomers.Count(e => e.JointId == id) > 0;
        }
    }
}