using ControleDocumentos.Models;
using ControleDocumentos.Repository;
using ControleDocumentos.Util;
using ControleDocumentos.Util.Extension;
using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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

                //if(model.UserName == "admin" && model.Password == "admin")
                //{
                //    Session.Add(EnumSession.Usuario.GetEnumDescription(), model);
                //    GetSessionUser();

                //    return Json(new { Status = true, Type = "success", ReturnUrl = Url.Action("Index", "Log") }, JsonRequestBehavior.AllowGet);
                //}

                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    UsuarioRepository usuarioRepository = new UsuarioRepository();
                    var usuario = usuarioRepository.GetUsuarioById(model.UserName);

                    CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                    serializeModel.IdUsuario = usuario.IdUsuario;
                    serializeModel.Nome = usuario.Nome;
                    serializeModel.Email = usuario.E_mail;
                    serializeModel.Permissao = usuario.Permissao;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();

                    string userData = serializer.Serialize(serializeModel);

                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                             1,
                             model.UserName,
                             DateTime.Now,
                             DateTime.Now.AddMinutes(30),
                             model.RememberMe,
                             userData);

                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    Response.Cookies.Add(faCookie);

                    if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Json(new { Status = true, Type = "success", ReturnUrl = returnUrl }, JsonRequestBehavior.AllowGet);
                    }
                    try
                    {
                        if (string.IsNullOrEmpty(usuario.E_mail))
                            return Json(new { Status = true, Type = "success", ReturnUrl = Url.Action("DadosCadastrais", "Home") }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception e)
                    {
                        //throw new Exception("Usuário ou senha inválidos");
                    }

                    return Json(new { Status = true, Type = "success", ReturnUrl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    throw new Exception("Usuário ou senha inválidos");
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
