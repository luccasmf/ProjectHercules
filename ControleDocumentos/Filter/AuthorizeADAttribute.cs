using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ControleDocumentos.Filter
{
    public class AuthorizeADAttribute : AuthorizeAttribute
    {
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