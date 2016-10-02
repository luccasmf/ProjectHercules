using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentos.Repository;
using ControleDocumentosLibrary;

namespace ControleDocumentos.Controllers
{
    public class CursoController : Controller
    {
        CursoRepository cursoRepository = new CursoRepository();
        // GET: Curso
        public ActionResult Index()
        {            
            return View(cursoRepository.GetCursos());
        }

        public object SalvaCurso(Curso curso) //serve pra cadastrar e editar
        {
            switch (cursoRepository.PersisteCurso(curso))
            {
                case "Cadastrado":
                    return Json(new { Status = true, Type = "success", Message = "Curso cadastrado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                case "Alterado":
                    return Json(new { Status = true, Type = "success", Message = "Curso alterado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                case "Erro":
                    return Json(new { Status = false, Type = "error", Message = "" }, JsonRequestBehavior.AllowGet);
                default:
                    return Json(new { Status = false, Type = "error", Message = "" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}