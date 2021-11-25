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
    public class CompanySignatoriesController : Controller
    {
        private readonly AppDbContext _db;

        public CompanySignatoriesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/CompanySignatories
        [HttpGet]
        public IQueryable<CompanySignatory> GetCompanySignatories()
        {
            return _db.CompanySignatories;
        }

        // GET: api/CompanySignatories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanySignatory(int id)
        {
            CompanySignatory companySignatory = await _db.CompanySignatories.FindAsync(id);
            if (companySignatory == null)
            {
                return NotFound();
            }

            return Ok(companySignatory);
        }

        [HttpGet("corporate/{id}")]
        public IActionResult GetCompanySignatoryByCorporate(int id)
        {
            return Ok(_db.CompanySignatories.Where(a => a.CorporateCustId == id));
        }

        // PUT: api/CompanySignatories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanySignatory(int id, CompanySignatory companySignatory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companySignatory.SignatoryId)
            {
                return BadRequest();
            }

            companySignatory.CreatedUserId = (_db.CompanySignatories.Where(a => a.SignatoryId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            companySignatory.CreatedDate = (_db.CompanySignatories.Where(a => a.SignatoryId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(companySignatory).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanySignatoryExists(id))
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

        // POST: api/CompanySignatories
        [HttpPost]
        public async Task<IActionResult> PostCompanySignatory([FromBody] CompanySignatory companySignatory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.CompanySignatories.Add(companySignatory);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Company Signatory has been added" });
        }

        // DELETE: api/CompanySignatories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanySignatory(int id)
        {
            CompanySignatory companySignatory = await _db.CompanySignatories.FindAsync(id);
            if (companySignatory == null)
            {
                return NotFound();
            }

            _db.CompanySignatories.Remove(companySignatory);
            await _db.SaveChangesAsync();

            return Ok(companySignatory);
        }

        [NonAction]
        private bool CompanySignatoryExists(int id)
        {
            return _db.CompanySignatories.Count(e => e.SignatoryId == id) > 0;
        }
    }
}