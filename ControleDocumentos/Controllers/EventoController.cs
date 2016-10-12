using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentos.Repository;
using ControleDocumentosLibrary;

namespace ControleDocumentos.Controllers
{
    public class EventoController : Controller
    {
        EventoRepository eventoRepository = new EventoRepository();
        // GET: Evento
        public ActionResult Index()
        {
           List<Evento> eventos = eventoRepository.GetEventos();
            return View(eventos);
        }

        public object SalvaEvento(Evento e) //serve pra cadastrar e editar
        {
            switch (eventoRepository.PersisteEvento(e))
            {
                case "Cadastrado":
                    return Json(new { Status = true, Type = "success", Message = "Evento cadastrado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                case "Alterado":
                    return Json(new { Status = true, Type = "success", Message = "Evento alterado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                case "Erro":
                    return Json(new { Status = false, Type = "error", Message = "" }, JsonRequestBehavior.AllowGet);
                default:
                    return Json(new { Status = false, Type = "error", Message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public object GeraCertificados(int idEvento)
        {
            bool flag = DirDoc.GeraCertificado(idEvento);

            if (flag)
            {
                return Json(new { Status = true, Type = "success", Message = "Certificados gerados!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Type = "error", Message = "Houve um erro, tente novamente mais tarde!" }, JsonRequestBehavior.AllowGet);

        }
    }
}