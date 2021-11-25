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
    [Route("api/Sessions")]
    public class SessionManagerController : Controller
    {
        private readonly AppDbContext _db;

        public SessionManagerController(AppDbContext db)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpGet]
        public IQueryable<SessionManager> GetSession()
        {
            IQueryable<SessionManager> session = _db.SessionManager;
            foreach (SessionManager sessionmanager in session)
            {
                sessionmanager.BranchName = (_db.BranchDetails.Where(a => a.BranchId == sessionmanager.BranchId).Select(b => b.BranchName)).FirstOrDefault();
            }
            return _db.SessionManager;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetSession(int id)
        {
            var session= _db.SessionManager.Where(a=>a.BranchId==id);
            foreach(SessionManager sessionmanager in session)
            {
                sessionmanager.BranchName = (_db.BranchDetails.Where(a => a.BranchId == id).Select(b => b.BranchName)).FirstOrDefault();
            }
            return Ok(session);
        }

        [AllowAnonymous]
        [HttpGet("active/{id}")]
        public IActionResult GetActiveSession(int id)
        {
            var session = _db.SessionManager.FirstOrDefault(a => a.BranchId == id && a.Status.ToLower().Contains("active"));
            session.BranchName = (_db.BranchDetails.Where(a => a.BranchId == id).Select(b => b.BranchName)).FirstOrDefault();
            return Ok(session);
        }

        [HttpPost]
        public async Task<IActionResult> AddSession(int id, [FromBody] SessionManager model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(_db.SessionManager.Any(a=>a.SessionDate==model.SessionDate && a.BranchId == model.BranchId))
            {
                return BadRequest("Session already exists in the same branch");
            }
            _db.SessionManager.Add(model);
            await _db.SaveChangesAsync();

            return Ok(new { Status = "OK", Message = "Successfully Added", Output = "Session has been added" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSession(int id,[FromBody] SessionManager model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != model.SessionManagerId)
            {
                return BadRequest();
            }
            if (_db.SessionManager.Any(a => a.SessionDate == model.SessionDate && a.BranchId == model.BranchId))
            {
                return BadRequest("Session already exists in the same branch");
            }

            model.CreatedUserId = (_db.SessionManager.Where(a => a.SessionManagerId == id).Select(a => a.CreatedUserId)).FirstOrDefault();
            model.CreatedDate = (_db.SessionManager.Where(a => a.SessionManagerId == id).Select(a => a.CreatedDate)).FirstOrDefault();
            _db.Entry(model).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionExists(id))
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

        [HttpPut("closebysessionid/{sessionid}")]
        public async Task<IActionResult> CloseSession(int sessionid)
        {
            if (_db.SessionManager.Any(a => a.SessionManagerId == sessionid))
            {
                var session = _db.SessionManager.Find(sessionid);

                session.ClosedDate=DateTime.Now;
                _db.Entry(session).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return Ok("Session Closed Successfully");
            }
            return BadRequest("Session not found");
        }

        [HttpPut("closebybranchid/{branchid}")]
        public async Task<IActionResult> CloseSessionbyBranch(int branchid)
        {
            if (_db.SessionManager.Any(a => a.SessionManagerId == branchid && a.Status.ToLower() == "active"))
            {
                var session = _db.SessionManager.FirstOrDefault(a => a.SessionManagerId == branchid && a.Status.ToLower() == "active");

                session.ClosedDate = DateTime.Now;
                _db.Entry(session).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return Ok("Session Closed Successfully");
            }
            return BadRequest("Session not found");
        }

        [NonAction]
        private bool SessionExists(int id)
        {
            return _db.SessionManager.Count(e => e.SessionManagerId == id) > 0;
        }
    }
}