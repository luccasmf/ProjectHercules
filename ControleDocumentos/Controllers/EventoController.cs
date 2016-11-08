using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentos.Repository;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;
using ControleDocumentos.Filter;
using ControleDocumentos.Util.Extension;

namespace ControleDocumentos.Controllers
{
    //[AuthorizeAD(Groups = "G_FACULDADE_ALUNOS, G_FACULDADE_PROFESSOR_R, G_FACULDADE_PROFESSOR_RW, G_FACULDADE_COORDENADOR_R, G_FACULDADE_COORDENADOR_RW, G_FACULDADE_SECRETARIA_R, G_FACULDADE_SECRETARIA_RW")]
    public class EventoController : BaseController
    {
        EventoRepository eventoRepository = new EventoRepository();
        CursoRepository cursoRepository = new CursoRepository();

        // GET: Evento
        public ActionResult Index()
        {
            PopularDropDownsFiltro();
            List<Evento> eventos = eventoRepository.GetEventos();

            return View(eventos);
        }

        //[AuthorizeAD(Groups = "G_FACULDADE_ALUNOS")]
        public ActionResult MeusEventos()
        {
            List<Evento> eventos = eventoRepository.GetEventosByAluno(Utilidades.UsuarioLogado.IdUsuario);

            return View(eventos);
        }

        public ActionResult CadastrarEvento(int? idEvento)
        {
            PopularDropDownsCadastro();
            Evento evento = new Evento();

            if (idEvento.HasValue)
            {
                evento = eventoRepository.GetEventoById((int)idEvento);
            }
            //retorna model
            return PartialView("_CadastroEvento", evento);
        }

        public ActionResult List(Models.EventoFilter filter)
        {
            return PartialView("_List", eventoRepository.GetByFilter(filter));
        }

        public ActionResult CarregaModalConfirmacao(EnumStatusEvento novoStatus, int idEvento)
        {
            Evento evento = new Evento { IdEvento = idEvento, Status = novoStatus };
            return PartialView("_AlteracaoStatus", evento);
        }

        public object SalvaEvento(Evento e, int[] Cursos) //serve pra cadastrar e editar
        {
            //lucciros aqui vai ter q percorrer e popular o evento com os cursos e tals

            switch (eventoRepository.PersisteEvento(e))
            {
                case "Mantido":
                    return Json(new { Status = true, Type = "success", Message = "Evento salvo com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                case "Cadastrado":
                    Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Persistir, e, null);
                    return Json(new { Status = true, Type = "success", Message = "Evento cadastrado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                case "Alterado":
                    Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Persistir, e, e.IdEvento);
                    return Json(new { Status = true, Type = "success", Message = "Evento alterado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                case "Erro":
                    return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação" }, JsonRequestBehavior.AllowGet);
                default:
                    return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação" }, JsonRequestBehavior.AllowGet);
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

        public ActionResult Chamada(int idEvento)
        {
            List<Aluno> alunos = eventoRepository.GetListaChamada(idEvento);
            return View(alunos);
        }

        public object FazerChamada(int[] idAlunos, int idEvento)
        {
            //id dos alunos presentes :)
            bool flag = eventoRepository.AdicionaPresenca(idAlunos, idEvento);

            if (flag)
            {
                return Json(new { Status = true, Type = "success", Message = "Chamada concluída!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Type = "error", Message = "Houve um erro, tente novamente mais tarde!" }, JsonRequestBehavior.AllowGet);
        }

        #region Métodos auxiliares
        private void PopularDropDownsFiltro()
        {
            var listStatus = Enum.GetValues(typeof(EnumStatusEvento)).
                Cast<EnumStatusEvento>().Select(v => new SelectListItem
                {
                    Text = EnumExtensions.GetEnumDescription(v),
                    Value = ((int)v).ToString(),
                }).ToList();

            ViewBag.Status = new SelectList(listStatus, "Value", "Text");
        }

        private void PopularDropDownsCadastro()
        {
            var listCursosSelectList = cursoRepository.GetCursos().Select(item => new SelectListItem
            {
                Value = item.IdCurso.ToString(),
                Text = item.Nome.ToString(),
            });
            ViewBag.Cursos = new SelectList(listCursosSelectList, "Value", "Text");
        }

        #endregion
    }
}