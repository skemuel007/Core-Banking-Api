using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AliasWebApiCore.Models;

namespace AliasWebApiCore
{
    public class Config
    {

        private static List<LogDetails> ConcurrentUsers = new List<LogDetails>();

        public static int? GetConcurrentUsers()
        {
            return ConcurrentUsers.Any()? ConcurrentUsers.Count:0;
        }

        public static List<LogDetails> GetConcurrentUsersList()
        {
            return  ConcurrentUsers.ToList();
        }

        public static string GetToken(string username)
        {
            return (ConcurrentUsers.Where(a=>a.Username==username).Select(b=>b.Token)).FirstOrDefault();
        }

        public static bool AddConcurrentUsers(string cuser,DateTime? lastlogout,string token)
        {
            if (ConcurrentUsers.Any(a=>a.Username==cuser))
            {
                return false;
            }
            else
            {
                LogDetails userlog = new LogDetails
                {
                    Username = cuser,
                    LastLogOut = lastlogout,
                    LogInTime = DateTime.Now.ToString("T"),
                    Token = token
                };
                ConcurrentUsers.Add(userlog);
                return true;
            }
        }

        public static bool RemoveConcurrentUsers(string cuser)
        {
            if (ConcurrentUsers.Any())
            {
                var user = ConcurrentUsers.FirstOrDefault(a => a.Username == cuser);
                if (user != null)
                {
                    ConcurrentUsers.Remove(user);
                    return true;
                }
            }
            return false;
        }

    }

}

public class LogDetails
{
    public string Username { get; set; }
    public DateTime? LastLogOut { get; set; }
    public string LogInTime { get; set; }
    public string Token { get; set; }
}
