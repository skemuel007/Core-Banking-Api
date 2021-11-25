using AliasWebApiCore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MainGeneralLedgerCodesController : Controller
    {
        private readonly AppDbContext _db;

        public MainGeneralLedgerCodesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/maingeneralledgercodes
        [HttpGet]
        public IQueryable<MainGeneralLedgerCodes> GetMainGlCodes()
        {
            return _db.MainGeneralLedgerCodes;
        }

        // GET: api/maingeneralledgercodes/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMainGlCodes(int id)
        {
            MainGeneralLedgerCodes mainGlCodes = await _db.MainGeneralLedgerCodes.FindAsync(id);
            if (mainGlCodes == null)
            {
                return NotFound();
            }

            return Ok(mainGlCodes);
        }

        // PUT: api/maingeneralledgercodes/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutMainGlCode(int id,[FromBody] MainGeneralLedgerCodes mainGlCodes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mainGlCodes.MainGeneralLedgerCodeId)
            {
                return BadRequest();
            }

            mainGlCodes.CreatedUserId = (_db.MainGeneralLedgerCodes.Where(a => a.MainGeneralLedgerCodeId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            mainGlCodes.CreatedDate = (_db.MainGeneralLedgerCodes.Where(a => a.MainGeneralLedgerCodeId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(mainGlCodes).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MainGlCodeExists(id))
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

        // POST: api/maingeneralledgercodes
        [HttpPost]
        public async Task<IActionResult> PostMainGlCode([FromBody] MainGeneralLedgerCodes mainGlCodes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.MainGeneralLedgerCodes.Add(mainGlCodes);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MainGlCodeExists(mainGlCodes.MainGeneralLedgerCodeId))
                {
                    return BadRequest();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "MainGlCode has been added" });
        }

        // DELETE: api/maingeneralledgercodes/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUserGroup(int id)
        {
            MainGeneralLedgerCodes mainGlCodes = await _db.MainGeneralLedgerCodes.FindAsync(id);
            if (mainGlCodes == null)
            {
                return NotFound();
            }

            _db.MainGeneralLedgerCodes.Remove(mainGlCodes);
            await _db.SaveChangesAsync();

            return Ok(mainGlCodes);
        }

        private bool MainGlCodeExists(int id)
        {
            return _db.MainGeneralLedgerCodes.Count(e => e.MainGeneralLedgerCodeId == id) > 0;
        }
    }
}
