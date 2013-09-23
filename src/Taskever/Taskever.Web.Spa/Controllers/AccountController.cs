﻿using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Abp.Exceptions;
using Abp.Modules.Core.Authorization;
using Abp.Modules.Core.Services;
using Abp.Web.Mvc.Controllers;
using Taskever.Web.Models;

namespace Taskever.Web.Controllers
{
    public class AccountController : AbpController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                if (!Membership.ValidateUser(loginModel.EmailAddress, loginModel.Password))
                {
                    throw new UserFriendlyException("No user name or password!");
                }

                //var user = _userService.GetUserOrNull(loginModel.EmailAddress, loginModel.Password);
                
                FormsAuthentication.SetAuthCookie(loginModel.EmailAddress, loginModel.RememberMe);

                //var authTicket = new FormsAuthenticationTicket(1, loginModel.EmailAddress, DateTime.Now, DateTime.Now.AddMinutes(15), false, "admin|deneme");
                //var encTicket = FormsAuthentication.Encrypt(authTicket);
                //var  faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                //Response.Cookies.Add(faCookie);

                return Redirect("/"); //TODO: Implement Return URL!
            }

            return View();
        }

        [AbpAuthorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
