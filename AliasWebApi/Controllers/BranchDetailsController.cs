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
    public class BranchDetailsController : Controller
    {
        private readonly AppDbContext _db;

        public BranchDetailsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/BranchDetails
        [AllowAnonymous]
        [HttpGet]
        public IQueryable<BranchDetails> GetBranchDetails()
        {
            return _db.BranchDetails;
        }

        // GET: api/BranchDetails/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchDetails(int id)
        {
            BranchDetails branchDetails = await _db.BranchDetails.FindAsync(id);
            if (branchDetails == null)
            {
                return NotFound();
            }

            return Ok(branchDetails);
        }

        // PUT: api/BranchDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranchDetails(int id,[FromBody] BranchDetails branchDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != branchDetails.BranchId)
            {
                return BadRequest();
            }

            branchDetails.CreatedUserId = (_db.BranchDetails.Where(a => a.BranchId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            branchDetails.CreatedDate = (_db.BranchDetails.Where(a => a.BranchId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(branchDetails).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchDetailsExists(id))
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

        // POST: api/BranchDetails
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostBranchDetails([FromBody] BranchDetails branchDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.BranchDetails.Add(branchDetails);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Branch has been added" });
        }

        // DELETE: api/BranchDetails/5
        [HttpDelete]
        public async Task<IActionResult> DeleteBranchDetails(int id)
        {
            BranchDetails branchDetails = await _db.BranchDetails.FindAsync(id);
            if (branchDetails == null)
            {
                return NotFound();
            }

            _db.BranchDetails.Remove(branchDetails);
            await _db.SaveChangesAsync();

            return Ok(branchDetails);
        }

        [NonAction]
        private bool BranchDetailsExists(int id)
        {
            return _db.BranchDetails.Count(e => e.BranchId == id) > 0;
        }
    }
}