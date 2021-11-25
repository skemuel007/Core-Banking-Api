using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public int UserCount { get; set; } = 0;
        public List<ApplicationUser> LoggedInUsers { get; set; }
        

        public IndexModel(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public void OnGet()
        {
            UserCount = _userManager.Users.Count(a => a.isLoggedIn);
            LoggedInUsers = _userManager.Users.Where(a => a.isLoggedIn).ToList();
        }

        public async Task<IActionResult> OnPost()
        {
            string username=HttpContext.Request.Query["data"].ToString();
            var appUser = _userManager.Users.FirstOrDefault(a => a.UserName.ToLower() == username.ToLower());
            if (appUser != null)
            {
                appUser.LastLogOut = DateTime.Now;
                appUser.isLoggedIn = false;
                await _userManager.UpdateAsync(appUser);
            }
            return RedirectToPage();
        }
    }
}