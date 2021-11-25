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
    public class GeneralLedgerCodeController : Controller
    {
        private readonly AppDbContext _db;

        public GeneralLedgerCodeController(AppDbContext db)
        {
            _db = db;
        }


        // GET: api/GeneralLedgerCodes
        [HttpGet]
        public IQueryable<GeneralLedgerCode> GetGLCode()
        {
            return _db.GeneralLedgerCodes;
        }

        // GET: api/GeneralLedgerCodes/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetGLCode(int id)
        {
            GeneralLedgerCode glcode = await _db.GeneralLedgerCodes.FindAsync(id);
            if (glcode == null)
            {
                return NotFound();
            }

            return Ok(glcode);
        }

        //notifyharmony codes instead of code
        // GET: api/GeneralLedgerCodes/cashier/5
        [HttpGet("cashier")]
        public  IActionResult GetGlCodeCashier()
        {
            return Ok(_db.GeneralLedgerCodes.Where(a => a.GLType.ToLower()== "teller"));
        }


        // PUT: api/GeneralLedgerCodes/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutGLCode(int id,[FromBody] GeneralLedgerCode generalledgercode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            if (id != generalledgercode.GeneralLedgerCodeId)
            {
                return BadRequest();
            }

            generalledgercode.CreatedUserId = (_db.GeneralLedgerCodes.Where(a => a.GeneralLedgerCodeId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            generalledgercode.CreatedDate = (_db.GeneralLedgerCodes.Where(a => a.GeneralLedgerCodeId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(generalledgercode).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeneralLedgerCodeExists(id))
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

        // POST: api/GeneralLedgerCodes
        [HttpPost]
        public async Task<IActionResult> PostGLCode([FromBody] GeneralLedgerCode generalLegderCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.GeneralLedgerCodes.Add(generalLegderCode);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "GLCode has been added" });
        }

        // DELETE: api/GeneralLedgerCodes/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteGLCode(int id)
        {
            GeneralLedgerCode glcode = await _db.GeneralLedgerCodes.FindAsync(id);
            if (glcode == null)
            {
                return NotFound();
            }

            _db.GeneralLedgerCodes.Remove(glcode);
            await _db.SaveChangesAsync();

            return Ok(glcode);  
        }

        [NonAction]
        private bool GeneralLedgerCodeExists(int id)
        {
            return _db.GeneralLedgerCodes.Count(e => e.GeneralLedgerCodeId == id) > 0;
        }
    }
}
