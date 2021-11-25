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
    public class LoansController : Controller
    {
        private readonly AppDbContext _db;

        public LoansController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Loans
        [HttpGet]
        public IQueryable<Loan> GetLoans()
        {
            return _db.Loans;
        }

        // GET: api/Loans/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLoan(int id)
        {
            Loan loan = await _db.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            return Ok(loan);
        }

        // PUT: api/Loans/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutLoan(int id,[FromBody] Loan loan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != loan.LoanId)
            {
                return BadRequest();
            }

            loan.CreatedUserId = (_db.Loans.Where(a => a.LoanId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            loan.CreatedDate = (_db.Loans.Where(a => a.LoanId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(loan).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(id))
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

        // POST: api/Loans
        [HttpPost]
        public async Task<IActionResult> PostLoan([FromBody] Loan loan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Loans.Add(loan);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Loans has been added" });
        }

        // DELETE: api/Loans/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            Loan loan = await _db.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            _db.Loans.Remove(loan);
            await _db.SaveChangesAsync();

            return Ok(loan);
        }

        [NonAction]
        private bool LoanExists(int id)
        {
            return _db.Loans.Count(e => e.LoanId == id) > 0;
        }
    }
}