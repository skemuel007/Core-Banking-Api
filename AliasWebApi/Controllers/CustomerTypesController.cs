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
    public class CustomerTypesController : Controller
    {
        private readonly AppDbContext _db;

        public CustomerTypesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/CustomerTypes
        [HttpGet]
        public IQueryable<CustomerType> GetCustomerTypes()
        {
            return _db.CustomerTypes;
        }

        // GET: api/CustomerTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerType(int id)
        {
            CustomerType customerType = await _db.CustomerTypes.FindAsync(id);
            if (customerType == null)
            {
                return NotFound();
            }

            return Ok(customerType);
        }

        // PUT: api/CustomerTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomerType(int id,[FromBody] CustomerType customerType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerType.CustomerTypeId)
            {
                return BadRequest();
            }

            customerType.CreatedUserId = (_db.CustomerTypes.Where(a => a.CustomerTypeId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            customerType.CreatedDate = (_db.CustomerTypes.Where(a => a.CustomerTypeId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(customerType).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerTypeExists(id))
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

        // POST: api/CustomerTypes
        [HttpPost]
        public async Task<IActionResult> PostCustomerType([FromBody] CustomerType customerType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.CustomerTypes.Add(customerType);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Customer Types has been added" });
        }

        // DELETE: api/CustomerTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerType(int id)
        {
            CustomerType customerType = await _db.CustomerTypes.FindAsync(id);
            if (customerType == null)
            {
                return NotFound();
            }

            _db.CustomerTypes.Remove(customerType);
            await _db.SaveChangesAsync();

            return Ok(customerType);
        }

        [NonAction]
        private bool CustomerTypeExists(int id)
        {
            return _db.CustomerTypes.Count(e => e.CustomerTypeId == id) > 0;
        }
    }
}