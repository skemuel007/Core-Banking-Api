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
    public class IssuedChequesController : Controller
    {
        private readonly AppDbContext _db;

        public IssuedChequesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/IssuedCheques
        [HttpGet]
        public IQueryable<IssuedChequeBooks> GetIssuedChequeBooks()
        {
            return _db.IssuedChequeBooks.Include(a=>a.Account);
        }

        // GET: api/IssuedCheques/5
        [HttpGet("{id:int}")]
        public async Task<object> GetIssuedChequeBook(int id)
        {
            IssuedChequeBooks issuedChequeBook = await _db.IssuedChequeBooks.FindAsync(id);
            if (issuedChequeBook == null)
            {
                return NotFound();
            }
            return (_db.IssuedChequeBooks.Where(a => a.IssuedChequeBookId == id).Include(k => k.Account).FirstOrDefault());
            //return Ok(issuedChequeBook);
        }

        // PUT: api/IssuedCheques/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutIssuedChequeBook(int id,[FromBody] IssuedChequeBooks issuedChequeBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != issuedChequeBook.IssuedChequeBookId)
            {
                return BadRequest();
            }

            issuedChequeBook.CreatedUserId = (_db.IssuedChequeBooks.Where(a => a.IssuedChequeBookId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            issuedChequeBook.CreatedDate = (_db.IssuedChequeBooks.Where(a => a.IssuedChequeBookId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(issuedChequeBook).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssuedChequeExists(id))
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

        // POST: api/IssuedCheques
        [HttpPost]
        public async Task<IActionResult> PostIssuedChequeBook([FromBody] IssuedChequeBooks issuedChequeBook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _db.IssuedChequeBooks.Add(issuedChequeBook);
            await _db.SaveChangesAsync();
            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Issued Cheque has been added" });
        }

        // DELETE: api/IssuedCheques/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteIssuedCheque(int id)
        {
            IssuedChequeBooks issuedChequeBook = await _db.IssuedChequeBooks.FindAsync(id);
            if (issuedChequeBook == null)
            {
                return NotFound();
            }

            _db.IssuedChequeBooks.Remove(issuedChequeBook);
            await _db.SaveChangesAsync();

            return Ok(issuedChequeBook);
        }

        [NonAction]
        private bool IssuedChequeExists(int id)
        {
            return _db.IssuedChequeBooks.Count(e => e.IssuedChequeBookId == id) > 0;
        }
    }
}

