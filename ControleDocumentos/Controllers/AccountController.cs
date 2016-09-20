using ControleDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ControleDocumentos.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            FormsAuthentication.SignOut();

            var model = new LoginModel();

            model.ReturnUrl = returnUrl;

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            string returnUrl = model.ReturnUrl;

            #region login teste
            if(model.UserName == "admin" && model.Password == "admin")
            {
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return this.Redirect(returnUrl);
                }

                return this.RedirectToAction("Index", "Home");
            }
            #endregion

            #region login Real
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(model);
                }

                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return this.Redirect(returnUrl);
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Erro = ex.Message;
            }
            #endregion

            return this.View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return this.RedirectToAction("Login", "Account");
        }
    }
}
