using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliasWebApiCore.Signal_R
{
    public interface IUserInfoInMemory
    {
        int GetCount();
        bool AddUpdate(string name, string connectionId);
        void Remove(string name);
        IEnumerable<UserInfo> GetAllUsersExceptThis(string username);
        UserInfo GetUserInfo(string username);
    }
    public class UserInfo
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
    }
}
