using ControleDocumentos.Models;
using ControleDocumentos.Util;
using ControleDocumentos.Util.Extension;
using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleDocumentos.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {

        }

        public Usuario GetSessionUser()
        {
            if (Utilidades.UsuarioLogado == null)
                try
                {
                    Utilidades.UsuarioLogado =(Usuario)Session[EnumSession.Usuario.GetEnumDescription()];
                }
                catch
                {
                    Utilidades.UsuarioLogado = Utilidades.GetSession((LoginModel)Session[EnumSession.Usuario.GetEnumDescription()]);
                }
            return Utilidades.UsuarioLogado;
        }

    }
}