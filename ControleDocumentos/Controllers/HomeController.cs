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
    [AuthorizeAD(Groups = "G_FACULDADE_ALUNOS, G_FACULDADE_PROFESSOR_R, G_FACULDADE_PROFESSOR_RW, G_FACULDADE_COORDENADOR_R, G_FACULDADE_COORDENADOR_RW, G_FACULDADE_SECRETARIA_R, G_FACULDADE_SECRETARIA_RW")]
    public class HomeController : BaseController
    {
        UsuarioRepository usuarioRepository = new UsuarioRepository();
        // GET: Home
        public ActionResult Index()
        {
            // PUBLICADO
            //Utilidades.UsuarioLogado = GetSessionUser();

            // LOCAL
            GetSessionUser();
            return View(Utilidades.UsuarioLogado);
        }
        
    }
}