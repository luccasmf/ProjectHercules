using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace ControleDocumentos.Util
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }

        public string IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public EnumPermissaoUsuario Permissao { get; set; }
    }
}