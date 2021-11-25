using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/ApprovalRules")]
    public class ApprovalRulesController : Controller
    {
        private readonly AppDbContext _db;

        public ApprovalRulesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IQueryable<ApprovalRules> GetApprovalRules()
        {
            return _db.ApprovalRules;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutApprovalRule(int id, [FromBody] ApprovalRules approvalRules)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != approvalRules.BranchId)
            {
                return BadRequest();
            }

            approvalRules.CreatedUserId = (_db.ApprovalRules.Where(a => a.ApprovalRulesId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            approvalRules.CreatedDate = (_db.ApprovalRules.Where(a => a.ApprovalRulesId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(approvalRules).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApprovalRuleExists(id))
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

        [HttpPost]
        public async Task<IActionResult> PostApprovalRules([FromBody] ApprovalRules approvalRules)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.ApprovalRules.Add(approvalRules);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Approval Rule has been added" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteApprovalRules(int id)
        {
            var approvalRule = await _db.ApprovalRules.FindAsync(id);
            if (approvalRule == null)
            {
                return NotFound();
            }

            _db.ApprovalRules.Remove(approvalRule);
            await _db.SaveChangesAsync();

            return Ok(approvalRule);
        }

        [NonAction]
        private bool ApprovalRuleExists(int id)
        {
            return _db.ApprovalRules.Count(e => e.ApprovalRulesId == id) > 0;
        }
    }
}