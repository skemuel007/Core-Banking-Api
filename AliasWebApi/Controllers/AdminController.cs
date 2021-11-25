using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public AdminController(UserManager<ApplicationUser> usrMgr, AppDbContext db)
        {
            _userManager = usrMgr;
            _db = db;
        }

        [HttpGet("api/claims/user/{username}")]
        public async Task<IEnumerable<string>> GetClaimsForUser(string username)
        {
            ApplicationUser appUser = _userManager.Users.FirstOrDefault(a => a.UserName == username);
            return (await _userManager.GetClaimsAsync(appUser)).Select(a=>a.Type);
        }

        [HttpPost("api/claims/user/remove")]
        public async Task<IActionResult> RemoveClaim([FromBody] ClaimsModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = _userManager.Users.FirstOrDefault(a => a.UserName == model.Username);
                var claims = new List<Claim>();
                foreach (string claimtype in model.Claims)
                {
                    claims.Add(new Claim(claimtype, "true"));
                }
                IdentityResult result = await _userManager.RemoveClaimsAsync(appUser,
                    claims);
                if (result.Succeeded)
                {
                    return Ok("Claim successfully removed from user");
                }
                else
                {
                    AddErrorsFromResult(result);
                    return BadRequest(ModelState);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("api/claims/user/add")]
        public async Task<IActionResult> AssignClaim([FromBody] ClaimsModel model)
        {
            if (ModelState.IsValid)
            {
                bool claimexists = false;
                ApplicationUser appUser = _userManager.Users.FirstOrDefault(a => a.UserName == model.Username);
                var claims = new List<Claim>();
                foreach (string claimtype in model.Claims)
                {
                    claims.Add(new Claim(claimtype, "true"));
                    
                    if ((await _userManager.GetClaimsAsync(appUser)).Count(a=>a.Type== claimtype)>0)//User.HasClaim(a=>a.Type==claimtype))
                    {
                        claimexists = true;
                    }
                }
                if (!claimexists)
                {
                    IdentityResult result = await _userManager.AddClaimsAsync(appUser, claims);
                    if (result.Succeeded)
                    {
                        if (model.Claims.Count > 1)
                        {
                            return Ok("Claims successfully added");
                        }
                        return Ok("Claim successfully added");
                    }
                    AddErrorsFromResult(result);
                    return BadRequest(ModelState);
                }
                else
                {
                    return BadRequest("Claims already exist");
                }
            }
            return BadRequest(ModelState);
        }

        [NonAction]
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

       
    }
    public class ClaimsModel
    {
        public string Username { get; set; }
        public string ClaimType { get; set; }
        public List<string> Claims { get; set; }
    }
}