using ControleDocumentos.Models;
using ControleDocumentos.Util.Extension;
using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ControleDocumentos.Controllers
{
    /// <summary>
    /// Controller para manipulação de informações gerais de contas (login, senha)
    /// </summary>
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

        /// <summary>
        /// Método responsável pela autenticação
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            string returnUrl = model.ReturnUrl;

            #region loginDiretoDev
            LoginModel lm = new LoginModel();
            lm.UserName = "admin";
            Session.Add(EnumSession.Usuario.GetEnumDescription(), lm);


            return this.RedirectToAction("Index", "Home");
            #endregion

            #region login teste
            //if(model.UserName == "admin" && model.Password == "admin")
            //{
            //    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

            //    if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
            //        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            //    {
            //        return this.Redirect(returnUrl);
            //    }

            //    return this.RedirectToAction("Index", "Home");
            //}
            #endregion

            #region login Real
            //try
            //{
            //    if (!this.ModelState.IsValid)
            //    {
            //        return this.View(model);
            //    }

            //    if (Membership.ValidateUser(model.UserName, model.Password))
            //    {
            //        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
            //        Session.Add(EnumSession.Usuario.GetEnumDescription(), model);

            //        if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
            //            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            //        {
            //            return this.Redirect(returnUrl);
            //        }

            //        return this.RedirectToAction("Index", "Home");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.Erro = ex.Message;
            //}
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
