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
        UsuarioRepository usuarioRepository = new UsuarioRepository();
        AlunoRepository alunoRepository = new AlunoRepository();

        // GET: Evento
        public ActionResult Index()
        {
            PopularDropDownsFiltro();
            List<Evento> eventos = eventoRepository.GetEventos();

            return View(eventos);
        }

        public ActionResult CadastrarEvento(int? idEvento)
        {
            PopularDropDownsCadastro(idEvento);
            Evento evento = new Evento();

            if (idEvento.HasValue)
            {
                evento = eventoRepository.GetEventoById((int)idEvento);
                evento.SelectedCursos = evento.Curso.Select(x => x.IdCurso).ToList();
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

        public object SalvaEvento(Evento e, int[] SelectedCursos) //serve pra cadastrar e editar
        {
            if (SelectedCursos == null)
                return Json(new { Status = false, Type = "error", Message = "Selecione pelo menos um curso." }, JsonRequestBehavior.AllowGet);

            if (e.IdEvento == 0)
            {
                Funcionario f = usuarioRepository.GetFuncionarioByUsuario(Utilidades.UsuarioLogado.IdUsuario);
                e.Status = EnumStatusEvento.ativo;
                e.IdFuncionarioCriador = f.IdFuncionario;
            }
            switch (eventoRepository.PersisteEvento(e, SelectedCursos))
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

        #region Lado do aluno
        //[AuthorizeAD(Groups = "G_FACULDADE_ALUNOS")]
        public ActionResult MeusEventos()
        {
            
            List<Evento> eventos = eventoRepository.GetEventoDisponivelByMeuCurso(Utilidades.UsuarioLogado.IdUsuario);

            return View(eventos);
        }

        public ActionResult ListAluno(Models.EventoFilter filter)
        {
            // lucciros adicionei um campo bool no filtro chamado "Apenas Inscritos", 
            // preciso que vc continue considerando o filtro por nome
            return PartialView("_List", eventoRepository.GetByFilterAluno(Utilidades.UsuarioLogado.IdUsuario, filter));
            //return PartialView("_List", new List<Evento>());
        }

        public ActionResult CarregaModalConfirmacaoParticipacao(int idEvento, bool presente)
        {
            Evento ev = eventoRepository.GetEventoById(idEvento);
            ViewBag.Presente = presente;
            return PartialView("_ConfirmacaoPresenca", ev);
        }

        public object ConfirmaCancelaPartipacao(Evento evento, bool Presente)
        {
            try
            {
                var ev = eventoRepository.GetEventoById(evento.IdEvento);
                var aluno = alunoRepository.GetAlunoByIdUsuario(Utilidades.UsuarioLogado.IdUsuario);
                bool ok = false;

                if (aluno != null)
                {
                    if (Presente)
                        ok = eventoRepository.InscreveAluno(aluno.IdAluno, evento.IdEvento);
                    else
                        ok = eventoRepository.DesinscreverAluno(aluno.IdAluno, evento.IdEvento);
                }

                if(ok)
                    return Json(new { Status = true, Type = "success", Message = "Alteração realizada com sucesso!" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

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

        private void PopularDropDownsCadastro(int? idEvento)
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