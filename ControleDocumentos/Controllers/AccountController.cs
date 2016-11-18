using ControleDocumentos.Models;
using ControleDocumentos.Util;
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
    public class AccountController : BaseController
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
            //LoginModel lm = new LoginModel();
            //lm.UserName = "admin";
            //Session.Add(EnumSession.Usuario.GetEnumDescription(), lm);

            //GetSessionUser();
            //return Json(new { Status = true, Type = "success", ReturnUrl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);


            #endregion

            #region login teste
            //if(model.UserName == "admin" && model.Password == "admin")
            //{
            //    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

            //    if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
            //        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            //    {
            //        // tem q testar esse returnurl pra ver se ta vindo certo
            //        return Json(new { Status = true, Type = "success", ReturnUrl = returnUrl}, JsonRequestBehavior.AllowGet);
            //    }
            //    return Json(new { Status = true, Type = "success", ReturnUrl = Url.Action("Index", "Home")}, JsonRequestBehavior.AllowGet);
            //}
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
                    Session.Add(EnumSession.Usuario.GetEnumDescription(), model);

                    if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        GetSessionUser();
                        return Json(new { Status = true, Type = "success", ReturnUrl = returnUrl }, JsonRequestBehavior.AllowGet);
                    }
                    try
                    {
                        GetSessionUser();
                        if (string.IsNullOrEmpty(Utilidades.UsuarioLogado.E_mail))
                            return Json(new { Status = true, Type = "success", ReturnUrl = Url.Action("DadosCadastrais", "Home") }, JsonRequestBehavior.AllowGet);
                    }
                    catch(Exception e)
                    {

                    }
                    


                    return Json(new { Status = true, Type = "success", ReturnUrl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Usuário inválido");
                }

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Type = "error", Message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
            #endregion
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return this.RedirectToAction("Login", "Account");
        }
    }
}
