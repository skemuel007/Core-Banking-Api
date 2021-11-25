using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AliasWebApiCore.Signal_R
{
    [Authorize]
    public class Message:Hub
    {
        private readonly IUserInfoInMemory _userInfoInMemory;

        public Message(IUserInfoInMemory userInfoInMemory)
        {
            _userInfoInMemory = userInfoInMemory;
        }

        public override async Task OnConnectedAsync()
        {
            
            await Clients.All.InvokeAsync("SendAction", Context.User.Identity.Name, "joined");
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Clients.All.InvokeAsync("SendAction", Context.User.Identity.Name, "left");
        }

        public async Task Leave()
        {
            _userInfoInMemory.Remove(Context.User.Identity.Name);
            await Clients.AllExcept(new List<string> { Context.ConnectionId }).InvokeAsync(
                "UserLeft",
                Context.User.Identity.Name
           );
        }

        public async Task ConnectedUsersCount()
        {
            int a = _userInfoInMemory.GetCount();
            await Clients.All.InvokeAsync("Count", a);
        }

        public async Task Join()
        {
            if (!_userInfoInMemory.AddUpdate(Context.User.Identity.Name, Context.ConnectionId))
            {
                // new user

                var list = _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name).ToList();
                await Clients.AllExcept(new List<string> { Context.ConnectionId }).InvokeAsync(
                    "NewOnlineUser",
                    _userInfoInMemory.GetUserInfo(Context.User.Identity.Name)
                );
            }
            else
            {
                // existing user joined again

            }

            await Clients.Client(Context.ConnectionId).InvokeAsync(
                "Joined",
                _userInfoInMemory.GetUserInfo(Context.User.Identity.Name)
            );

            await Clients.Client(Context.ConnectionId).InvokeAsync(
                "OnlineUsers",
                _userInfoInMemory.GetAllUsersExceptThis(Context.User.Identity.Name)
            );
        }

        public Task SendDirectMessage(string message, string targetUserName)
        {
            var userInfoSender = _userInfoInMemory.GetUserInfo(Context.User.Identity.Name);
            var userInfoReciever = _userInfoInMemory.GetUserInfo(targetUserName);
            return Clients.Client(userInfoReciever.ConnectionId).InvokeAsync("SendDM", message, userInfoSender);
        }

        public void Send(string name, string message)
        {
            Clients.All.InvokeAsync("broadcastMessage", name, message);
        }
    }
}
