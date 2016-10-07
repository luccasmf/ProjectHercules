﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;
using ControleDocumentos.Util.Extension;

namespace ControleDocumentos.Filter
{
    public class AuthorizeADAttribute : AuthorizeAttribute
    {
        DocumentosModel db = new DocumentosModel();
        public string Groups { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                if (httpContext.User.Identity.Name == "admin")
                {
                    return true;
                }


                if (string.IsNullOrEmpty(Groups))
                    return true;

                var groups = Groups.Split(',').ToList();

                var context = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["Dominio"], "9077401526", "12qw!@QW"); //usuario com direitos q nao entendi...

                var userPrincipal = UserPrincipal.FindByIdentity(
                                       context,
                                       IdentityType.SamAccountName,
                                       httpContext.User.Identity.Name);

                var perm = userPrincipal.GetGroups();   //ver os grupos que o usuario participa

                try
                {
                    Usuario user = db.Usuario.Find(httpContext.User.Identity.Name);

                    if (user == null)
                    {
                        user = new Usuario();
                        Funcionario f;
                        Aluno al;

                        user.IdUsuario = httpContext.User.Identity.Name;
                        user.Nome = userPrincipal.Name;

                        if (userPrincipal.IsMemberOf(context, IdentityType.Name, "G_FACULDADE_COORDENADOR_R") || userPrincipal.IsMemberOf(context, IdentityType.Name, "G_FACULDADE_COORDENADOR_RW"))
                        {
                            user.Permissao = EnumPermissaoUsuario.coordenador;
                            f = new Funcionario();
                            f.IdUsuario = user.IdUsuario;
                            f.Permissao = EnumPermissaoUsuario.coordenador;
                            db.Funcionario.Add(f);
                        }
                        else if (userPrincipal.IsMemberOf(context, IdentityType.Name, "G_FACULDADE_PROFESSOR_R") || userPrincipal.IsMemberOf(context, IdentityType.Name, "G_FACULDADE_PROFESSOR_RW"))
                        {
                            user.Permissao = EnumPermissaoUsuario.professor;
                            f = new Funcionario();
                            f.IdUsuario = user.IdUsuario;
                            f.Permissao = EnumPermissaoUsuario.professor;
                            db.Funcionario.Add(f);
                        }
                        else if (userPrincipal.IsMemberOf(context, IdentityType.Name, "G_FACULDADE_SECRETARIA_R") || userPrincipal.IsMemberOf(context, IdentityType.Name, "G_FACULDADE_SECRETARIA_RW"))
                        {
                            user.Permissao = EnumPermissaoUsuario.secretaria;
                            f = new Funcionario();
                            f.IdUsuario = user.IdUsuario;
                            f.Permissao = EnumPermissaoUsuario.secretaria;
                            db.Funcionario.Add(f);
                        }
                        else
                        {
                            user.Permissao = EnumPermissaoUsuario.aluno;
                            al = new Aluno();
                            al.IdUsuario = user.IdUsuario;
                            db.Aluno.Add(al);
                        }
                        db.Usuario.Add(user);

                        db.SaveChanges();

                        httpContext.Session[EnumSession.Usuario.GetEnumDescription()] = user;
                    }
                }
                catch
                {
                    return false;
                }

                foreach (var group in groups)
                    if (userPrincipal.IsMemberOf(context,
                         IdentityType.Name,
                         group))
                        return true;
            }
            return false;
        }



        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Error/NotAuthorized" }));
            }
        }

    }
}