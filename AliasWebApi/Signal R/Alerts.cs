using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AliasWebApiCore.Signal_R
{
   [AllowAnonymous]
    public class Alerts:Hub
    {
        private readonly IUserInfoInMemory _userInfoInMemory;

        public Alerts(IUserInfoInMemory userInfoInMemory)
        {
            _userInfoInMemory = userInfoInMemory;
        }


        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "joined");
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.SendAsync("SendAction", Context.User.Identity.Name, "left");
        }

        public async Task Leave()
        {
            _userInfoInMemory.Remove(Context.User.Identity.Name);
            await Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync(
                "UserLeft",
                Context.User.Identity.Name
           );
           
        }

        public async Task Join()
        {
            if (!_userInfoInMemory.AddUpdate(Context.User.Identity.Name, Context.ConnectionId))
            {
                // new user

                var list = _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name).ToList();
                await Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync(
                    "NewOnlineUser",
                    _userInfoInMemory.GetUserInfo(Context.User.Identity.Name)
                );
            }
            else
            {
                // existing user joined again

            }

            await Clients.Client(Context.ConnectionId).SendAsync(
                "Joined",
                _userInfoInMemory.GetUserInfo(Context.User.Identity.Name)
            );

            await Clients.Client(Context.ConnectionId).SendAsync(
                "OnlineUsers",
                _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name)
            );
        }

        public Task SendDirectMessage(string message, string targetUserName)
        {
            var userInfoSender = _userInfoInMemory.GetUserInfo(Context.User.Identity.Name);
            var userInfoReciever = _userInfoInMemory.GetUserInfo(targetUserName);
            return Clients.Client(userInfoReciever.ConnectionId).SendAsync("SendDM", message, userInfoSender);
        }
        
        public Task Send(string user, string message)
        {
            string timestamp = DateTime.Now.ToShortTimeString();
            return Clients.All.SendAsync("Send", timestamp, user, message);
        }
    }
}
