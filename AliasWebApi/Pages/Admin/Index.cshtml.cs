using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using AliasWebApiCore.Models;
using Microsoft.EntityFrameworkCore;

namespace AliasWebApiCore.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly AslBankingDbContext _db;
        public int UserCount { get; set; } = 0;
        public List<LogDetails> LoggedInUsers { get; set; }

        public IndexModel(AslBankingDbContext db)
        {
            _db = db;
        }

        [AllowAnonymous]
        public void OnGet()
        {
            UserCount = Config.GetConcurrentUsers() ?? 0;
            LoggedInUsers = Config.GetConcurrentUsersList();
        }

        [AllowAnonymous]
        public IActionResult OnPost()
        {
            string username=HttpContext.Request.Query["data"].ToString();
            var user = _db.Users.FirstOrDefault(a => a.Username.ToLower() == username.ToLower());
            user.LastLogOut = DateTime.Now;
            _db.Entry(user).State = EntityState.Modified;
            _db.SaveChanges();
            Config.RemoveConcurrentUsers(username.ToLower());
            return RedirectToPage();
        }
    }
}