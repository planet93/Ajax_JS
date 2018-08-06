using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ajax_JS.Models;

namespace Ajax_JS.Controllers
{
    public class HomeController : Controller
    {
        static ChatModel chatModel;

        public ActionResult Index(string user,bool? logOn,bool? logOff,string chatMessage)
        {
            try
            {
                if(chatModel==null)
                {
                    chatModel = new ChatModel();
                }

                if(chatModel.Messages.Count>20)
                {
                    chatModel.Messages.RemoveRange(0, 10);
                }

                if (!Request.IsAjaxRequest())
                {
                    return View(chatModel);
                }
                else if (logOn != null && (bool)logOn)
                {
                    if (chatModel.Users.FirstOrDefault(u => u.Name == user) != null)
                    {
                        throw new Exception("такой пользователь уже есть");
                    }
                    else if (chatModel.Users.Count > 10)
                    {
                        throw new Exception("В чате слишком много людей");
                    }
                    else
                    {
                        chatModel.Users.Add(new ChatUser()
                        {
                            Name = user,
                            LoginTime = DateTime.Now,
                            LastPing = DateTime.Now
                        });


                        chatModel.Messages.Add(new ChatMessage()
                        {
                            Text = user + "вошел в чат",
                            Date = DateTime.Now
                        });
                    }
                    return PartialView("ChatRoom", chatModel);
                }
                else if (logOff != null && (bool)logOff)
                {
                    LogOff(chatModel.Users.FirstOrDefault(u => u.Name == user));
                    return PartialView("ChatRoom", chatModel);
                }
                else
                {
                    ChatUser currentUser = chatModel.Users.FirstOrDefault(u => u.Name == user);
                    currentUser.LastPing = DateTime.Now;

                    if (!string.IsNullOrEmpty(chatMessage))
                    {
                        chatModel.Messages.Add(new ChatMessage()
                        {
                            User = currentUser,
                            Text = chatMessage,
                            Date = DateTime.Now
                        });
                    }

                    return PartialView("History", chatModel);
                }
                                
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Content(ex.Message);
            }
        }
        
        public void LogOff(ChatUser user)
        {
            chatModel.Users.Remove(user);
            chatModel.Messages.Add(new ChatMessage()
            {
                Text = user.Name + "покинул чат.",
                Date = DateTime.Now
            });
        }
    }
}