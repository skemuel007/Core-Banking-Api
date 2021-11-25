using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AliasWebApi.Models;
using AliasWebApiCore.Extensions;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using AliasWebApiCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AliasWebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class UserAccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _db;
        private readonly ILogger _logger;
        //private readonly IEmailSender _emailSender;
        //private readonly IDataProtector _protector;

        public UserAccountsController(UserManager<ApplicationUser> usrMgr, SignInManager<ApplicationUser> signinMgr,
            IConfiguration configuration, AppDbContext db, ILogger<UserAccountsController> logger/*,IEmailSender emailSender, IDataProtectionProvider provider*/)
        {
            _userManager = usrMgr;
            _signInManager = signinMgr;
            _configuration = configuration;
            _db = db;
            _logger = logger;
            //_emailSender = emailSender;
            //_protector = provider.CreateProtector("Contoso.MyClass.v1");
        }

        [AllowAnonymous]
        [HttpPost("login",Name = "Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (_userManager.Users.Count(a => a.isLoggedIn.Equals(true)) > 9)
            {
                return BadRequest("Only ten concurrent users are allowed.");
            }
            ApplicationUser appUser = await _userManager.FindByNameAsync(model.Username);
            User user = _db.User.FirstOrDefault(a => a.Username == model.Username);

            if (user == null) return BadRequest("Invalid Username or Password");
            if (await _userManager.IsLockedOutAsync(appUser))
            {
                return BadRequest("Too many login attempts. You have been locked out of your account.Try again in 3 minutes.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(appUser, model.Password, true);

            if (!result.Succeeded) return BadRequest("Invalid Username or Password");
            var userClaims = await _userManager.GetClaimsAsync(appUser);
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, appUser.UserName));
            userClaims.Add(new Claim(ClaimTypes.Name, model.Username));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qwertyuiopasdfghjklzxcvbnm123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "http://localhost:65300",
                audience: "http://localhost:65300",
                claims: userClaims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: creds);
            appUser.isLoggedIn = true;
            appUser.LoginTime=DateTime.Now;
            await _userManager.UpdateAsync(appUser);
           
            var msg = $"User {appUser.UserName} logged in";
            _logger.LogInformation(msg);
            return Ok(new LoginReturn
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token), Expires_In_Hours = 12,
                BranchId = Convert.ToInt32(user.BranchId),
                UserId = user.UserId,
                LastLogout = appUser.LastLogOut?? null,
                FullName=$"{user.FirstName} {user.LastName}",
                SessionDate = Convert.ToDateTime((_db.SessionManager
                .Where(a=>a.BranchId==user.BranchId && a.Status=="active").Select(a=>a.SessionDate)).FirstOrDefault()),
                Claims = (await _userManager.GetClaimsAsync(appUser)).Select(a => a.Type)
            });
            //return Ok(new { access_token=tokenresult/*access_token = new JwtSecurityTokenHandler().WriteToken(token)*/, expires_in_hours = 6, User = _db.Users.FirstOrDefault(a => a.Username == model.Username) });
        }

        public class LoginReturn
        {
            public string Access_Token { get; set; }
            public int Expires_In_Hours { get; set; }
            public int UserId { get; set; }
            public string FullName { get; set; }
            public int BranchId { get; set; }
            public DateTime SessionDate { get; set; }
            public DateTime? LastLogout { get; set; } 
            public IEnumerable<string> Claims { get; set; }
            
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var appUser = _userManager.Users.FirstOrDefault(a => a.UserName.ToLower() == User.Identity.Name.ToLower());
            if (appUser != null)
            {
                appUser.isLoggedIn = false;
                appUser.LastLogOut = DateTime.Now;
                await _userManager.UpdateAsync(appUser);
            }
            return Ok("Logout successfull");
        }

        [HttpPost("unlock")]
        public async Task<IActionResult> UnlockApp([FromBody] UnlockModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            var appUser = await _userManager.Users.FirstOrDefaultAsync(a => a.UserName.ToLower() == User.Identity.Name.ToLower());
            string passwordhash = _userManager.PasswordHasher.HashPassword(appUser, model.Password);
            if (passwordhash == appUser.PasswordHash)
            {
                return Ok(/*new { access_token = Config.GetToken(User.Identity.Name.ToLower()) }*/);
            }
            return BadRequest("Invalid Password");
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> PasswordChange([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var appUser = await _userManager.FindByNameAsync(model.Username);
            //var user = _db.Users.FirstOrDefault(a => a.Username == model.Username);
            if (appUser == null /*|| user==null*/) return NotFound("User does not exist");
            string passwordhash = _userManager.PasswordHasher.HashPassword(appUser, model.OldPassword);
            if (passwordhash != appUser.PasswordHash) return BadRequest("Current Password is incorrect.");

            IdentityResult result = await _userManager.ChangePasswordAsync(appUser, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                //user.Password = model.NewPassword;
                //_db.Entry(user).State = EntityState.Modified;
                //_db.SaveChanges();
                return Ok("Password Changed Successfully");
            }
            else
            {
                return BadRequest("Could not change Password");
            }
        }

        //[HttpPost("resetpassword")]
        //public async Task<IActionResult> PasswordReset([FromBody] ChangePasswordModel model)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var appuser = await _userManager.FindByNameAsync(model.Username);
        //    var user = _db.Users.FirstOrDefault(a => a.Username == model.Username);
        //    if (appuser == null || user == null) return NotFound("User does not exist");
        //    if (model.OldPassword != user.Password) return BadRequest("Current Password is incorrect.");

        //    var code = await _userManager.GeneratePasswordResetTokenAsync(appuser);
        //    var callbackUrl = Url.ResetPasswordCallbackLink(appuser.Id, code, Request.Scheme);
        //    await _emailSender.SendResetPasswordAsync(appuser.Email, callbackUrl);
        //    return Ok($"Password reset email sent to {appuser.Email}");
        //}
    }

    public class ChangePasswordModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required()]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UnlockModel
    {
        [Required]
        public string Password { get; set; }
    }
}
