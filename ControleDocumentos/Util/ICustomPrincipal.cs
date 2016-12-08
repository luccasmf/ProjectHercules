using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ControleDocumentos.Util
{
    interface ICustomPrincipal : IPrincipal
    {
        string IdUsuario { get; set; }
        string Nome { get; set; }
        string Email { get; set; }
        EnumPermissaoUsuario Permissao { get; set; }
    }
}
