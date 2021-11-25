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
    public class CompanyDirectorsController : Controller
    {
        private readonly AppDbContext _db;

        public CompanyDirectorsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/CompanyDirectors
        [HttpGet]
        public IQueryable<CompanyDirectors> GetCompanyDirectorses()
        {
            return _db.CompanyDirectorses;
        }

        // GET: api/CompanyDirectors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyDirectors(int id)
        {
            CompanyDirectors companyDirectors = await _db.CompanyDirectorses.FindAsync(id);
            if (companyDirectors == null)
            {
                return NotFound();
            }

            return Ok(companyDirectors);
        }

        [HttpGet("corporate/{id}")]
        public IActionResult GetCompanySignatoryByCorporate(int id)
        {
            return Ok(_db.CompanyDirectorses.Where(a => a.CorporateCustId == id));
        }

        // PUT: api/CompanyDirectors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyDirectors(int id,[FromBody] CompanyDirectors companyDirectors)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companyDirectors.DirectorId)
            {
                return BadRequest();
            }

            companyDirectors.CreatedUserId = (_db.CompanyDirectorses.Where(a => a.DirectorId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            companyDirectors.CreatedDate = (_db.CompanyDirectorses.Where(a => a.DirectorId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(companyDirectors).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyDirectorsExists(id))
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

        // POST: api/CompanyDirectors
        [HttpPost]
        public async Task<IActionResult> PostCompanyDirectors([FromBody] CompanyDirectors companyDirectors)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.CompanyDirectorses.Add(companyDirectors);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Company Director has been added" });
        }

        // DELETE: api/CompanyDirectors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyDirectors(int id)
        {
            CompanyDirectors companyDirectors = await _db.CompanyDirectorses.FindAsync(id);
            if (companyDirectors == null)
            {
                return NotFound();
            }

            _db.CompanyDirectorses.Remove(companyDirectors);
            await _db.SaveChangesAsync();

            return Ok(companyDirectors);
        }

        [NonAction]
        private bool CompanyDirectorsExists(int id)
        {
            return _db.CompanyDirectorses.Count(e => e.DirectorId == id) > 0;
        }
    }
}