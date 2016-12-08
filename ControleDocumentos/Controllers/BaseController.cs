using ControleDocumentos.Filter;
using ControleDocumentos.Models;
using ControleDocumentos.Repository;
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
    //[AuthorizeAD(Groups = "G_PROTOCOLO_ADMIN, G_FACULDADE_ALUNOS, G_FACULDADE_PROFESSOR_R, G_FACULDADE_PROFESSOR_RW")]
    public abstract class BaseController : Controller
    {
        UsuarioRepository usuarioRepository = new UsuarioRepository();

        public BaseController()
        {

        }

        public Usuario GetSessionUser()
        {
            try
            {
                Utilidades.UsuarioLogado = (Usuario)Session[EnumSession.Usuario.GetEnumDescription()];
                Utilidades.UsuarioLogado = usuarioRepository.GetUsuarioById(Utilidades.UsuarioLogado.IdUsuario);
            }
            catch
            {
                Utilidades.UsuarioLogado = Utilidades.GetSession((LoginModel)Session[EnumSession.Usuario.GetEnumDescription()]);
                Utilidades.UsuarioLogado = usuarioRepository.GetUsuarioById(Utilidades.UsuarioLogado.IdUsuario);

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