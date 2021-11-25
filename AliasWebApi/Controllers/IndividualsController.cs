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
    public class IndividualsController : Controller
    {
        private readonly AppDbContext _db;

        public IndividualsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Individuals
        [HttpGet]
        public IQueryable<Individual> GetInviduals()
        {

            return _db.Individuals;
            //return db.Inviduals.Include(u => u.User);
        }
        //GET: api/Individuals/gender
        [HttpGet("{gender:alpha}")]
        public IQueryable<Individual> GetInviduals(string gender)
        {
            return _db.Individuals.Where(u => u.Gender.ToLower() == gender);
        }
        // GET: api/Individuals/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetIndividuals(int id)
        {
            Individual individual = await _db.Individuals.FindAsync(id);
            if (individual == null)
            {
                return NotFound();
            }

            return Ok(individual);
        }

        // PUT: api/Individuals/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutIndividual(int id,[FromBody] Individual individual)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != individual.IndividualCustId)
            {
                return BadRequest();
            }

            individual.CreatedUserId = (_db.Individuals.Where(a => a.IndividualCustId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            individual.CreatedDate = (_db.Individuals.Where(a => a.IndividualCustId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(individual).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IndividualExists(id))
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

        // POST: api/Individuals
        [HttpPost]
        public async Task<IActionResult> PostIndividual([FromBody] Individual individual)
        {
            var cust = _db.Individuals.Any(c => c.CustomerNumber.Equals(individual.CustomerNumber) && c.FirstName.Equals(individual.FirstName) && c.LastName.Equals(individual.LastName) && c.Gender.Equals(individual.Gender) && c.DateOfBirth.Equals(individual.DateOfBirth));
              if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            //if (!_db.CustomerTypes.Any(a => a.CustomerTypeId == individual.CustomerTypeId))
            //{
            //    return BadRequest("Invalid Customer Type ID");
            //}
                if (cust)
                {
                    return BadRequest("customer already exist");
                
                }
                _db.Individuals.Add(individual);

                await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Individual Customer has been added" });

        }

        // DELETE: api/Individuals/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteIndividual(int id)
        {
            Individual individual = await _db.Individuals.FindAsync(id);
            if (individual == null)
            {
                return NotFound();
            }

            _db.Individuals.Remove(individual);
            await _db.SaveChangesAsync();

            return Ok(individual);
        }

        [NonAction]
        private bool IndividualExists(int id)
        {
            return _db.Individuals.Count(e => e.IndividualCustId == id) > 0;
        }
    }
}