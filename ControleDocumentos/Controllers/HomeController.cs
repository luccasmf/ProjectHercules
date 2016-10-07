using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentos.Filter;

namespace ControleDocumentos.Controllers
{
    // [AuthorizeAD(Groups = "G_PROTOCOLO_ADMIN, G_FACULDADE_ALUNOS, G_FACULDADE_PROFESSOR_R, G_FACULDADE_PROFESSOR_RW")]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}