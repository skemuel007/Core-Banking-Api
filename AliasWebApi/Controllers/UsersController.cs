using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AliasWebApiCore.Extensions;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using AliasWebApiCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;
        //private readonly IEmailSender _emailSender;

        public UsersController(UserManager<ApplicationUser> usrMgr, AppDbContext db/*, IEmailSender emailSender*/)
        {
            _userManager = usrMgr;
            _db = db;
            //_emailSender = emailSender;
        }


        // GET: api/Users
        [AllowAnonymous]
        [HttpGet]
        public IQueryable<User> GetUsers()
        {
            return _db.User;
        }

        //GET: api/Users/uname
        [HttpGet("{uname:alpha}")]
        public IActionResult GetUser(string uname)
        {
            var usr = _db.User.Any(u => u.Username.Equals(uname));

            if (!usr)
            {
                return NotFound("User not found");
            }
            return Ok(_db.User.Where(u => u.Username.ToLower() == uname));
        }

        // GET: api/Users/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            User user = await _db.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (id != user.UserId)
                {
                    return BadRequest();
                }
                if (user.BranchId!=null)
                {
                    if (!_db.BranchDetails.Any(a => a.BranchId == user.BranchId))
                    {
                        return BadRequest("Incorrect branch id");
                    }
                }
                user.CreatedUserId = _db.User.Where(a=>a.UserId==id).Select(a=>a.CreatedUserId).FirstOrDefault();
                user.CreatedDate = _db.User.Where(a => a.UserId == id).Select(a => a.CreatedDate).FirstOrDefault();
                _db.Entry(user).State = EntityState.Modified;

                await _db.SaveChangesAsync();
                ApplicationUser appUser = await _userManager.FindByNameAsync(user.Username);
                appUser.Email = user.Email;
                //await _userManager.RemovePasswordAsync(appUser);
                //await _userManager.AddPasswordAsync(appUser, user.Password);
                //string passwordhash = _userManager.PasswordHasher.HashPassword(appUser, user.Password);
                //appUser.PasswordHash = passwordhash;
                await _userManager.UpdateNormalizedEmailAsync(appUser);
                await _userManager.UpdateNormalizedUserNameAsync(appUser);
                await _userManager.UpdateSecurityStampAsync(appUser);
                await _userManager.UpdateAsync(appUser);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!UserExists(id))
                {
                    return NotFound();

                }
                Console.WriteLine(ex.Message);

            }

            return NoContent();
        }

        // POST: api/Users
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_db.BranchDetails.Any(a => a.BranchId == user.BranchId))
            {
                return BadRequest("Incorrect branch id");
            }
            if (user.CreatedUserId != null)
            {
                if (!_db.Users.Any(a => a.UserId == user.CreatedUserId))
                {
                    return BadRequest("Created UserId does not exist");
                }
            }
            var usr = _db.Users.Any(u => u.UserName.Equals(user.Username));
            var appusr = _userManager.Users.Any(u => u.UserName.Equals(user.Username));
            if (usr || appusr)
            {
                return BadRequest("Username already taken");
            }
            _db.User.Add(user);
            await _db.SaveChangesAsync();
            ApplicationUser appUser = new ApplicationUser
            {
                UserName = user.Username,
                Email = user.Email,
                UserId = user.UserId
            };
            IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);

            if (!result.Succeeded)
            {
                return BadRequest("Incorrect Username or password");
            }
            else
            {
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                //var callbackUrl = Url.EmailConfirmationLink(appUser.Id, code, Request.Scheme);
                //await _emailSender.SendEmailConfirmationAsync(user.Email, callbackUrl);
                //return Ok("Email sent successfully");
                return Ok(new { Status = "OK", Message = "Successfully Added", Output ="User has been added" /*$"Confirm your email: {user.Email} to log in."*/ });
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            User user = await _db.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _db.User.Remove(user);
            await _db.SaveChangesAsync();
            ApplicationUser appUser = await _userManager.FindByNameAsync(user.Username);
            IdentityResult result = await _userManager.DeleteAsync(appUser);
            //if (!result.Succeeded)
            //{
            //    return BadRequest("Incorrect Username or password");
            //}
            return Ok(user);
        }

        [NonAction]
        private bool UserExists(int id)
        {
            return _db.Users.Count(e => e.UserId == id) > 0;
        }
    }
}