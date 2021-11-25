using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CountryListsController : Controller
    {
        private readonly AppDbContext _db;

        public CountryListsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/CountryLists
        [HttpGet]
        public IQueryable<CountryList> GetCountryLists()
        {
            return _db.CountryLists;
        }

        // GET: api/CountryLists/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCountryList(int id)
        {
            CountryList countryList = await _db.CountryLists.FindAsync(id);
            if (countryList == null)
            {
                return NotFound();
            }

            return Ok(countryList);
        }

        // PUT: api/CountryLists/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountryList(int id,[FromBody] CountryList countryList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != countryList.CountryId)
            {
                return BadRequest();
            }

            countryList.CreatedUserId = (_db.CountryLists.Where(a => a.CountryId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            countryList.CreatedDate = (_db.CountryLists.Where(a => a.CountryId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(countryList).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Successfully updated");
        }

        // POST: api/CountryLists
        [HttpPost]
        public async Task<IActionResult> PostCountryList([FromBody] CountryList countryList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.CountryLists.Add(countryList);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Country List has been added" });
        }

        // DELETE: api/CountryLists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountryList(int id)
        {
            CountryList countryList = await _db.CountryLists.FindAsync(id);
            if (countryList == null)
            {
                return NotFound();
            }

            _db.CountryLists.Remove(countryList);
            await _db.SaveChangesAsync();

            return Ok(countryList);
        }

        [NonAction]
        private bool CountryListExists(int id)
        {
            return _db.CountryLists.Count(e => e.CountryId == id) > 0;
        }
    }
}