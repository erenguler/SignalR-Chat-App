using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OnlyText.Web.Models;
using OnlyText.Web.MyCodes;

namespace OnlyText.Web.Controllers
{
    
    public class ChatController : Controller
    {
        // GET: Chat

        MessageBLL _messages = new MessageBLL();
        WebUserBLL _users = new WebUserBLL();

        [LoginControl]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetMessages(string senderUserName)
        {
            WebUser user = Session["User"] as WebUser;
            WebUser sender = _users.GetFirstOrDefault(x=>x.UserName == senderUserName);

            StringBuilder result = new StringBuilder();
            result.Append("");

            List<Message> incomingMessages = _messages.GetAll(x => x.Receiver_Id == user.Id && x.WebUser_Id == sender.Id);
            List<Message> outgoingMessages = _messages.GetAll(x => x.Receiver_Id == sender.Id && x.WebUser_Id == user.Id);

            List<Message> allMessages = incomingMessages;
            allMessages.AddRange(outgoingMessages);

            allMessages = allMessages.OrderBy(x => x.MessageDate).ToList();

            foreach (var item in allMessages)
            {

                if (item.WebUser_Id == user.Id) // message owner
                {
                    result.Append(@"<div class='main-message-box ta-right'>
                                <div class='message-dt'>
                                    <div class='message-inner-dt'>
                                        <p>"+item.MessageText+@"</p>
                                    </div>
                                    <span>"+item.MessageDate.ToShortTimeString()+@"</span>
                                </div>
                                <div class='messg-usr-img'>
                                    <img src='/Content/images/WebUsersPP/"+item.WebUser.Avatar+@"' alt=''>
                                </div>
                            </div>");
                }
                else // message from other person
                {
                    result.Append(@"<div class='main-message-box st3'>
                                <div class='message-dt st3'>
                                    <div class='message-inner-dt'>
                                        <p>"+item.MessageText+@"</p>
                                    </div>
                                    <span>"+item.MessageDate.ToShortTimeString()+ @"</span>
                                </div>
                                <div class='messg-usr-img'>
                                    <img src='/Content/images/WebUsersPP/" + item.WebUser.Avatar+@"' alt=''>
                                </div>
                            </div>");
                }
            }

            return Json(result.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}