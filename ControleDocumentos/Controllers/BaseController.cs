using ControleDocumentos.Models;
using ControleDocumentos.Util;
using ControleDocumentos.Util.Extension;
using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.IO;
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
            try
            {
                Utilidades.UsuarioLogado = (Usuario)Session[EnumSession.Usuario.GetEnumDescription()];
            }
            catch
            {
                Utilidades.UsuarioLogado = Utilidades.GetSession((LoginModel)Session[EnumSession.Usuario.GetEnumDescription()]);
            }
            return Utilidades.UsuarioLogado;
        }

        private string[] formatos = new string[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".bmp" };
        public bool ValidaArquivo(string nomeArquivo)
        {
            string extensao = Path.GetExtension(nomeArquivo);

            if (formatos.Contains(extensao))
                return true;
            else
                return false;

        }

    }
}