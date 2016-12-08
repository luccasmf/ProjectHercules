using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControleDocumentos.Util
{
    public class CustomPrincipalSerializeModel
    {
        public string IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public EnumPermissaoUsuario Permissao { get; set; }
    }
}