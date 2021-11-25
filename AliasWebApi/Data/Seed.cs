using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;
using AliasWebApiCore.Models.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AliasWebApiCore.Data
{
    public class Seed
    {
        public static async Task SetupData(IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            var db =
                serviceProvider.GetRequiredService<AppDbContext>();
            if (!db.Users.Any())
            {
                var user = new User
                {
                    Username = "admin",
                    Password = "admin"
                };
            }
        }
    }
}
