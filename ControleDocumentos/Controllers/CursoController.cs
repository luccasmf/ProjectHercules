using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentos.Repository;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;
using ControleDocumentos.Filter;

namespace ControleDocumentos.Controllers
{
    //[AuthorizeAD(Groups = "G_FACULDADE_ALUNOS, G_FACULDADE_PROFESSOR_R, G_FACULDADE_PROFESSOR_RW, G_FACULDADE_COORDENADOR_R, G_FACULDADE_COORDENADOR_RW, G_FACULDADE_SECRETARIA_R, G_FACULDADE_SECRETARIA_RW")]
    public class CursoController : BaseController
    {
        CursoRepository cursoRepository = new CursoRepository();
        // GET: Curso
        public ActionResult Index()
        {
            PopularDropDowns();
            return View(cursoRepository.GetCursos());
        }

        public ActionResult CarregaModalCadastro(int? idCurso)
        {
            //instancia model
            Curso curso = new Curso();

            if (idCurso.HasValue)
            {
                //pega model pelo id
                curso = cursoRepository.GetCursoById(idCurso.Value);
            }
            //retorna model
            return PartialView("_CadastroCurso", curso);
        }

        public ActionResult CarregaModalExclusao(int idCurso)
        {
            Curso curso = cursoRepository.GetCursoById(idCurso);

            return PartialView("_ExclusaoCurso", curso);
        }

        public object SalvarCurso(Curso curso) //serve pra cadastrar e editar
        {
            switch (cursoRepository.PersisteCurso(curso))
            {
                case "Cadastrado":
                    Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Persistir, curso, null);
                    return Json(new { Status = true, Type = "success", Message = "Curso cadastrado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                case "Alterado":
                    Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Persistir, curso, curso.IdCurso);
                    return Json(new { Status = true, Type = "success", Message = "Curso alterado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                case "Erro":
                    return Json(new { Status = false, Type = "error", Message = "" }, JsonRequestBehavior.AllowGet);
                default:
                    return Json(new { Status = false, Type = "error", Message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult List()
        {
            return PartialView("_List", cursoRepository.GetCursos());
        }

        public object ExcluirCurso(Curso curso)
        {
            // lucciros, tem que ver se precisa validar se tem aluno matriculado e impedir de excluir se tiver
            curso = cursoRepository.GetCursoById(curso.IdCurso);
            //if (cursoRepository.DeletaCurso(curso.IdCurso))
            //{
            //    Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Excluir, curso, curso.IdCurso);
            //    return Json(new { Status = true, Type = "success", Message = "Curso deletado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            //}
            return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AtualizarCoordenadores()
        {
            // lucciros, atualizar lista de coordenadores
            var lstCoordenadores = new List<Funcionario>();
            return Json(lstCoordenadores.Select(x => new { Value = x.IdUsuario, Text = x.Usuario.Nome }));
        }

        #region Métodos auxiliares

        private void PopularDropDowns()
        {
            // lucciros, buscar coordenadores
            var listCoordenadores = new List<Funcionario>().Select(item => new SelectListItem
            {
                Value = item.IdUsuario.ToString(),
                Text = item.Usuario.Nome.ToString(),
            });
            ViewBag.Coordenadores = new SelectList(listCoordenadores, "Value", "Text");
        }

        #endregion
    }
}