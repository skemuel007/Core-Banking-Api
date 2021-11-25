using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AliasWebApiCore.Models
{
    public class Setup
    {
        private readonly AslBankingDbContext _db;
        private readonly AppIdentityDbContext _appdb;
        private readonly UserManager<ApplicationUser> _userManager;

        public Setup(AslBankingDbContext db, AppIdentityDbContext appdb, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _appdb = appdb;
            _userManager = userManager;
        }

        //public static async  void Initialize(IServiceProvider serviceProvider)
        //{
        //    var context = serviceProvider.GetRequiredService<AslBankingDbContext>();
        //    var appContext = serviceProvider.GetRequiredService<AppIdentityDbContext>();
        //    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        //    context.Database.EnsureCreated();
            
        //    if (!appContext.Users.Any())
        //    {
        //        ApplicationUser appUser = new ApplicationUser
        //        {
        //            UserName = "admin"
        //        };
        //        IdentityResult result = await userManager.CreateAsync(appUser, "admin");

        //        if (result.Succeeded)
        //        {
        //            if (!context.Users.Any())
        //            {
        //                context.Users.Add(new User { Username = "admin", Password = "admin", Status = true, DateOfBirth = new DateTime() });
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //}

    }
}
