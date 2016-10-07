using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentos.Filter;
using ControleDocumentosLibrary;
using ControleDocumentos.Util.Extension;
using ControleDocumentos.Repository;
using ControleDocumentos.Models;
using ControleDocumentos.Util;

namespace ControleDocumentos.Controllers
{
    //[AuthorizeAD(Groups = "G_PROTOCOLO_ADMIN, G_FACULDADE_ALUNOS, G_FACULDADE_PROFESSOR_R, G_FACULDADE_PROFESSOR_RW")]
    public class HomeController : Controller
    {
        UsuarioRepository usuarioRepository = new UsuarioRepository();
        // GET: Home
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)Session[EnumSession.Usuario.GetEnumDescription()];
            
            return View(usuario);
        }

        
    }
}