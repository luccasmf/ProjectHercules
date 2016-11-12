using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentos.Repository;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;
using ControleDocumentos.Models;
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

        #region Lado do Coordenador/Secretaria

        public ActionResult Index()
        {
            // lucciros trazer apenas eventos relacionados com o curso que coordena
            PopularDropDownsFiltro();
            List<Evento> eventos = eventoRepository.GetByFilterCoord(Utilidades.UsuarioLogado.IdUsuario, new EventoFilter());

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

        public ActionResult List(EventoFilter filter)
        {
            // lucciros filtrar apenas eventos relacionados com o curso que coordena
            return PartialView("_List", eventoRepository.GetByFilterCoord(Utilidades.UsuarioLogado.IdUsuario, filter));
        }

        public ActionResult CarregaModalConfirmacao(EnumStatusEvento novoStatus, int idEvento)
        {
            Evento evento = new Evento { IdEvento = idEvento, Status = novoStatus };
            return PartialView("_AlteracaoStatus", evento);
        }

        public object SalvaEvento(Evento e, int[] SelectedCursos)
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
            // falta testar esse metodo depois que a chamada estiver funcionando
            bool flag = DirDoc.GeraCertificado(idEvento);

            if (flag)
            {
                return Json(new { Status = true, Type = "success", Message = "Certificados gerados com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Type = "error", Message = "Houve um erro, tente novamente mais tarde!" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Chamada(int idEvento)
        {
            // lucciros, preciso que vc me retorne a lista de de alunos inscritos do 
            // coordenador
            List<int> cursos = cursoRepository.GetCursoByCoordenador(Utilidades.UsuarioLogado.IdUsuario).Select(y=> y.IdCurso).ToList();

            List<Aluno> alunos = eventoRepository.GetListaChamada(idEvento).Where(x=> cursos.Contains(x.AlunoCurso.FirstOrDefault().IdCurso)).ToList();
            var evento = eventoRepository.GetEventoById(idEvento);

            //lucciros, preciso que vc pegue quantas chamadas foram realizadas pro evento
            //acho que é legal ter essa info pro coordenador não se perder se o evento
            //for de varios dias
            return PartialView("_Chamada",new ChamadaModel {
                Alunos = alunos,
                IdEvento = idEvento,
                NomeEvento = evento.NomeEvento,
                NumChamada = 1 // aqui vai o retorno do metodo + 1, se foram realizadas 0 chamadas, essa é a 1
            });
        }

        public object FazerChamada(int[] idAlunos, int idEvento)
        {
            bool flag = eventoRepository.AdicionaPresenca(idAlunos, idEvento);

            if (flag)
            {
                return Json(new { Status = true, Type = "success", Message = "Chamada concluída!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Type = "error", Message = "Houve um erro, tente novamente mais tarde!" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Lado do aluno
        //[AuthorizeAD(Groups = "G_FACULDADE_ALUNOS")]
        public ActionResult MeusEventos()
        {

            List<Evento> eventos = eventoRepository.GetByFilterAluno(Utilidades.UsuarioLogado.IdUsuario, new EventoFilter()).Where(x => DateTime.Now < x.DataFim).ToList();

            return View(eventos);
        }

        public ActionResult ListAluno(EventoFilter filter)
        {
            return PartialView("_List", eventoRepository.GetByFilterAluno(Utilidades.UsuarioLogado.IdUsuario, filter).Where(x => DateTime.Now < x.DataFim).ToList());
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

                if (ok)
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