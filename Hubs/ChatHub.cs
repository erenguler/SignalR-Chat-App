using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using OnlyText.Web.Controllers;
using OnlyText.Web.Models;
using OnlyText.Web.MyCodes;

namespace OnlyText.Web.Hubs
{

    public class ChatHub : Hub
    {
        static List<UserDetail> ConnectedUsers = new List<UserDetail>(); // list of connected users

        WebUserBLL _users = new WebUserBLL();
        MessageBLL _messages = new MessageBLL();

        public void Connect()
        {
            string username = Toolkit.GetCurrentWebUserName();
            WebUser user = _users.GetFirstOrDefault(x => x.UserName == username);
            user.Status = true;
            _users.Update(user);

            if (ConnectedUsers.Count(x => x.ConnectionId == user.ConnectionId) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = Context.ConnectionId, UserID = user.Id, UserName = user.UserName });
                Clients.Caller.onConnected(user.UserName);
                Clients.Others.onUserConnected(user.ConnectionId, user.UserName);
            }
        }

        public void SendPrivateMessage(string toUserName, string msg)
        {
            try
            {
                string fromconnectionid = Context.ConnectionId;
                var toUser = ConnectedUsers.FirstOrDefault(x => x.UserName == toUserName);
                string fromUserName = Toolkit.GetCurrentWebUserName();

                WebUser me = _users.GetFirstOrDefault(x => x.UserName == fromUserName);
                WebUser you = _users.GetFirstOrDefault(x => x.UserName == toUserName);

                // if the person we talk to is online, Send message and save to database
                if (toUser != null) 
                {
                    Clients.Client(toUser.ConnectionId).sendPrivateMessage(fromUserName, msg,DateTime.Now.ToShortTimeString(),me.Avatar); // karşı tarafa gönder
                    Clients.Caller.addMessage(fromUserName, msg, DateTime.Now.ToShortTimeString(),me.Avatar); // kendine gönder

                    Message newMessage = new Message();
                    newMessage.MessageDate = DateTime.Now;
                    newMessage.MessageText = msg;
                    newMessage.Receiver_Id = you.Id;
                    newMessage.WebUser_Id = me.Id;
                    _messages.InsertOrUpdate(newMessage);
                }
                // if the person we talk to is offline, save to database
                else
                {
                    Clients.Caller.addMessage(fromUserName, msg, DateTime.Now.ToShortTimeString(), me.Avatar); // kendine gönder

                    Message newMessage = new Message();
                    newMessage.MessageDate = DateTime.Now;
                    newMessage.MessageText = msg;
                    newMessage.Receiver_Id = you.Id;
                    newMessage.WebUser_Id = me.Id;
                    _messages.InsertOrUpdate(newMessage);
                }
            }
            catch { }
        }



        public void SetTyping(string toUserName)
        {
            try
            {
                string fromconnectionid = Context.ConnectionId;
                var toUser = ConnectedUsers.FirstOrDefault(x => x.UserName == toUserName);
                string fromUserName = Toolkit.GetCurrentWebUserName();

                if (toUser != null) // if the person we talk to is online
                {
                    Clients.Client(toUser.ConnectionId).setTyping(fromUserName); 
                }
            }
            catch { }
        }


        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                WebUser user = _users.GetFirstOrDefault(x => x.UserName == item.UserName);
                user.Status = false;
                _users.Update(user);

                var connectionId = Context.ConnectionId;
                Clients.Others.onUserDisconnected(connectionId, item.UserName);
            }

            return base.OnDisconnected(stopCalled);
        }

    }


    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}