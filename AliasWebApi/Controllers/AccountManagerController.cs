using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    public class AccountManagerController : Controller
    {
        private readonly AppDbContext _db;

        public AccountManagerController(AppDbContext db)
        {
            _db = db;
        }


        //Get for liens
        [HttpGet("api/lien")]
        public IQueryable<Liens> GetLiens()
        {
            return _db.Liens;
        }

        [HttpGet("api/lien/{id}")]
        public async Task<IActionResult> GetLien(int id)
        {
            Liens lien = await _db.Liens.FindAsync(id);
            if (lien == null)
            {
                return NotFound();
            }

            return Ok(lien);
        }

        //Get for sweep
        [HttpGet("api/sweep")]
        public IQueryable<Sweep> GetSweeps()
        {
            var sweeps = _db.Sweeps;
            foreach (var sweep in sweeps)
            {
                sweep.FromAccountNumber =
                    (_db.Accounts.Where(a => a.AccountId == sweep.FromAccountId).Select(b => b.AccountNumber))
                    .FirstOrDefault();
                sweep.ToAccountNumber =
                    (_db.Accounts.Where(a => a.AccountId == sweep.ToAccountId).Select(b => b.AccountNumber))
                    .FirstOrDefault();
            }
            return sweeps;
        }

        [HttpGet("api/sweep/{id}")]
        public async Task<IActionResult> GetSweep(int id)
        {
            Sweep sweep = await _db.Sweeps.FindAsync(id);
            if (sweep == null)
            {
                return NotFound();
            }
            sweep.FromAccountNumber =
                (_db.Accounts.Where(a => a.AccountId == sweep.FromAccountId).Select(b => b.AccountNumber))
                .FirstOrDefault();
            sweep.ToAccountNumber =
                (_db.Accounts.Where(a => a.AccountId == sweep.ToAccountId).Select(b => b.AccountNumber))
                .FirstOrDefault();
            return Ok(sweep);
        }

        //Get for cheque
        [HttpGet("api/cheque")]
        public IQueryable<Cheques> GetCheques()
        {
            return _db.Cheques.Include(i => i.Account);
        }

        [HttpGet("api/cheque/{id}")]
        public async Task<IActionResult> GetCheque(int id)
        {
            var cheques = _db.Cheques.Include(a => a.Account);
            var filtered = await cheques.FirstOrDefaultAsync(a => a.ChequeId == id);
            if (filtered == null)
            {
                return NotFound();
            }
            return Ok(filtered);
        }

        //Get for overdraft
        [HttpGet("api/overdraft")]
        public IQueryable<Overdrafts> GetOverdrafts()
        {
            return _db.Overdrafts;
        }

        [HttpGet("api/overdraft/{id}")]
        public async Task<IActionResult> GetOverdraft(int id)
        {
            Overdrafts overdraft = await _db.Overdrafts.FindAsync(id);
            if (overdraft == null)
            {
                return NotFound();
            }

            return Ok(overdraft);
        }

        [HttpPost("api/overdraft")]
        public async Task<IActionResult> PostOverdraft([FromBody] Overdrafts overdraft)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _db.Overdrafts.Add(overdraft);
            await _db.SaveChangesAsync();
            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Overdraft has been added" });
        }

        [HttpGet("api/accounts/pending")]
        public IQueryable<Account> GetInactiveAccount()
        {
            return _db.Accounts.Where(a => a.AccountStatus.ToLower().Contains("inactive")).Include(a => a.Individual).Include(b => b.Corporate).Include(c => c.JointCustomer);
        }

        [HttpGet("api/transcodeitems/pending")]
        public IQueryable<TransCodeItems> GetTransCodeItems()
        {
            return _db.TransCodeItems.Where(a=>a.Status.ToLower().Contains("pending")).Include(a=>a.Account.Individual).Include(b=>b.Account.Corporate).Include(c=>c.Account.JointCustomer);
        }
    }
}
