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
    public class LoanServicingsController : Controller
    {
        private readonly AppDbContext _db;

        public LoanServicingsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/LoanServicings
        [HttpGet]
        public IQueryable<LoanServicing> GetLoanServicings()
        {
            return _db.LoanServicings;
        }

        // GET: api/LoanServicings/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLoanServicing(int id)
        {
            LoanServicing loanServicing = await _db.LoanServicings.FindAsync(id);
            if (loanServicing == null)
            {
                return NotFound();
            }

            return Ok(loanServicing);
        }

        // PUT: api/LoanServicings/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutLoanServicing(int id,[FromBody] LoanServicing loanServicing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != loanServicing.LoanServicingCustId)
            {
                return BadRequest();
            }

            loanServicing.CreatedUserId = ((_db.LoanServicings.Where(a => a.LoanServicingCustId == id).Select(a => a.CreatedUserId)).FirstOrDefault());
            loanServicing.CreatedDate = (_db.LoanServicings.Where(a => a.LoanServicingCustId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(loanServicing).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanServicingExists(id))
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

        // POST: api/LoanServicings
        [HttpPost]
        public async Task<IActionResult> PostLoanServicing([FromBody] LoanServicing loanServicing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.LoanServicings.Add(loanServicing);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Loan Servicing has been added" });
        }

        // DELETE: api/LoanServicings/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLoanServicing(int id)
        {
            LoanServicing loanServicing = await _db.LoanServicings.FindAsync(id);
            if (loanServicing == null)
            {
                return NotFound();
            }

            _db.LoanServicings.Remove(loanServicing);
            await _db.SaveChangesAsync();

            return Ok(loanServicing);
        }

        [NonAction]
        private bool LoanServicingExists(int id)
        {
            return _db.LoanServicings.Count(e => e.LoanServicingCustId == id) > 0;
        }
    }
}