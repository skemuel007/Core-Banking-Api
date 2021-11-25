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
    public class TransactionsController : Controller
    {
        private readonly AppDbContext _db;

        public TransactionsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Transactions
        [HttpGet]
        public IEnumerable<Transaction> GetTransactions()
        {
            List<Transaction> trans = _db.Transactions.ToList();
            for (int i=0;i<trans.Count;i++)
            {
                if (trans[i].Debit > 0)
                {
                    trans[i].Balance =
                        i!=0 ? trans[i - 1].Balance - trans[i].Debit : trans[i].Balance - trans[i].Debit;
                }
                else if(trans[i].Credit > 0)
                {
                    trans[i].Balance =
                        i!=0 ? trans[i - 1].Balance + trans[i].Credit : trans[i].Balance + trans[i].Credit;
                }
            }
            return trans.OrderByDescending(a => a.TransactionId);
            
        }

       

        // GET: api/Transactions/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            Transaction transaction = await _db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpGet("account/{id}")]
        public IEnumerable<Transaction> GetTransactionByAccountId(int id)
        {
            List<Transaction> trans = _db.Transactions.Where(a => a.AccountId == id).ToList();
            
            for (int i = 0; i < trans.Count; i++)
            {
                if (trans[i].Debit > 0)
                {
                    trans[i].Balance =
                        i >1 ? trans[i - 2].Balance - trans[i].Debit : trans[i].Balance /*- trans[i].Debit*/;
                }
                else if (trans[i].Credit > 0)
                {
                    trans[i].Balance =
                        i >1 ? trans[i - 2].Balance + trans[i].Credit : trans[i].Balance /*+ trans[i].Credit*/;
                }
            }
            return trans.OrderByDescending(a => a.TransactionId);
        }


        // PUT: api/Transactions/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutTransaction(int id,[FromBody] Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }

            transaction.CreatedUserId = (_db.Transactions.Where(a => a.TransactionId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            transaction.CreatedDate = (_db.Transactions.Where(a => a.TransactionId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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

        // POST: api/Transactions
        [HttpPost]
        public async Task<IActionResult> PostTransaction([FromBody] Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Transaction has been added" });
        }

        // DELETE: api/Transactions/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            Transaction transaction = await _db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _db.Transactions.Remove(transaction);
            await _db.SaveChangesAsync();

            return Ok(transaction);
        }

        [NonAction]
        private bool TransactionExists(int id)
        {
            return _db.Transactions.Count(e => e.TransactionId == id) > 0;
        }
    }
}