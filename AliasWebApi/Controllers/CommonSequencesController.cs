using AliasWebApiCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ASLApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CommonSequencesController : Controller
    {
        private readonly AppDbContext _db;

        public CommonSequencesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/CommonSequences
        [HttpGet]
        public IQueryable<CommonSequence> GetCommonSequences()
        {
            return _db.CommonSequences;
        }

        // GET: api/CommonSequences/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommonSequence(int id)
        {
            CommonSequence commonSequence = await _db.CommonSequences.FindAsync(id);
            if (commonSequence == null)
            {
                return NotFound();
            }

            return Ok(commonSequence);
        }
        // PUT: api/CommonSequences/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommonSequence(int id,[FromBody] CommonSequence commonSequence)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != commonSequence.CommonSequenceId)
            {
                return BadRequest();
            }

            commonSequence.CreatedUserId = (_db.CommonSequences.Where(a => a.CommonSequenceId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            commonSequence.CreatedDate = (_db.CommonSequences.Where(a => a.CommonSequenceId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(commonSequence).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommonSequenceExists(id))
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

        //[HttpPut]
        [HttpPut("counter/{id}")]
        public async Task<IActionResult> PutCounter(int id,[FromBody] CommonSequence commonSequence)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            CommonSequence Commonsequence = await _db.CommonSequences.FindAsync(id);
            if (Commonsequence == null)
            {
                return BadRequest();
            }
            try
            {
                Commonsequence.Counter = commonSequence.Counter;
                _db.Entry(Commonsequence).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Ok(new { message = e.ToString() });
            }
            return Ok(new { message = "Counter Updated" });
        }

        // POST: api/CommonSequences
        [HttpPost]
        public async Task<IActionResult> PostCommonSequence([FromBody] CommonSequence commonSequence)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.CommonSequences.Add(commonSequence);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Common Sequence has been added" });
        }

        // DELETE: api/CommonSequences/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommonSequence(int id)
        {
            CommonSequence commonSequence = await _db.CommonSequences.FindAsync(id);
            if (commonSequence == null)
            {
                return NotFound();
            }

            _db.CommonSequences.Remove(commonSequence);
            await _db.SaveChangesAsync();

            return Ok(commonSequence);
        }

        [NonAction]
        private bool CommonSequenceExists(int id)
        {
            return _db.CommonSequences.Count(e => e.CommonSequenceId == id) > 0;
        }
    }
}