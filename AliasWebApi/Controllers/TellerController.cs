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
    public class TellerController : Controller
    {
        private readonly AppDbContext _db;

        public TellerController(AppDbContext db)
        {
            _db = db;
        }

       

        // GET: api/Tellers
        [HttpGet]
        public IQueryable<Teller> GetTellers()
        {
            return _db.Teller;
        }



        // GET: api/teller/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTeller(int id)
        {
            Teller teller = await _db.Teller.FindAsync(id);
            if (teller == null)
            {
                return NotFound();
            }

            return Ok(teller);
        }

        // GET: api/teller/5
        [HttpGet("user/{userid:int}")]
        public  IActionResult GetTellerByUserId(int userid)
        {
            var teller = _db.Teller.FirstOrDefault(a => a.CreatedUserId == userid);
            if (teller == null)
            {
                return NotFound();
            }

            return Ok(teller);
        }

        // PUT: api/teller/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutTeller(int id,[FromBody] Teller teller)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != teller.TellerID)
            {
                return BadRequest();
            }

            teller.CreatedUserId = (_db.Teller.Where(a => a.TellerID == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            teller.CreatedDate = (_db.Teller.Where(a => a.TellerID == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(teller).State = EntityState.Modified;
            List<Teller> tellers = _db.Teller.Where(a => a.BranchId == teller.BranchId).ToList();
            bool exists = false;
            foreach (Teller Teller in tellers)
            {
                if (Teller.GeneralLedgerCodeId == teller.GeneralLedgerCodeId) { exists = true; }
            }
            if (exists)
            {
                return BadRequest("Two tellers cannot have the same user id in one branch");
            }
            else
            {
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TellerExists(id))
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

           
        }

        // POST: api/Teller
        [HttpPost]
        public IActionResult PostTeller([FromBody] Teller teller)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Teller> tellers = _db.Teller.Where(a => a.BranchId == teller.BranchId ).ToList();
            bool exists = false;
            foreach(Teller Teller in tellers)
            {
                if (Teller.GeneralLedgerCodeId == teller.GeneralLedgerCodeId) { exists = true; }
            }
            if (exists)
            {
                return BadRequest("Two tellers cannot have the same user id in one branch");
            }
            else
            {
                    _db.Teller.Add(teller);
                    _db.SaveChanges();
            }

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Teller has been added" });
        }

        // DELETE: api/Teller/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTeller(int id)
        {
            Teller teller = await _db.Teller.FindAsync(id);
            if (teller == null)
            {
                return NotFound();
            }

            _db.Teller.Remove(teller);
            await _db.SaveChangesAsync();

            return Ok(teller);
        }
       
        [NonAction]
        public  bool TellerExists(int id)
        {
            return _db.Teller.Count(e => e.TellerID == id) > 0;
        }

    }
}
