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
        CursoRepository cursoRepository = new CursoRepository();
        AlunoRepository alunoRepository = new AlunoRepository();
        // GET: Home
        public ActionResult Index()
        {
            // PUBLICADO
            //Utilidades.UsuarioLogado = GetSessionUser();

            // LOCAL
            GetSessionUser();

            if (Utilidades.UsuarioLogado.Permissao == EnumPermissaoUsuario.aluno)
                return RedirectToAction("Index", "SolicitacaoDocumentoAluno");
            else if (Utilidades.UsuarioLogado.Permissao == EnumPermissaoUsuario.coordenador)
                return RedirectToAction("Index", "SolicitacaoHoraComplementar");
            else if (Utilidades.UsuarioLogado.Permissao == EnumPermissaoUsuario.professor)
                return RedirectToAction("Index", "Evento");
            else if (Utilidades.UsuarioLogado.Permissao == EnumPermissaoUsuario.secretaria)
                return RedirectToAction("Index", "Documento");

            return View(Utilidades.UsuarioLogado);
        }

        public ActionResult DadosCadastrais()
        {
            if (Utilidades.UsuarioLogado.Permissao == EnumPermissaoUsuario.aluno)
            {
                var aluno = alunoRepository.GetAlunoByIdUsuario(Utilidades.UsuarioLogado.IdUsuario);
                var idCurso = aluno.AlunoCurso != null ? aluno.AlunoCurso.Select(x => x.IdCurso).FirstOrDefault() : 0;

                var listCursosSelectList = cursoRepository.GetCursos().Select(item => new SelectListItem
                {
                    Value = item.IdCurso.ToString(),
                    Text = item.Nome.ToString(),
                });
                ViewBag.Cursos = new SelectList(listCursosSelectList, "Value", "Text", idCurso.ToString());
            }

            var usuario = usuarioRepository.GetUsuarioById(Utilidades.UsuarioLogado.IdUsuario);
            return View(usuario);
        }

        public object SalvarDadosCadastrais(Usuario usuario, int IdCurso)
        {
            try
            {
                bool flag = alunoRepository.PersisteAluno(usuario.IdUsuario, IdCurso);
                
                
                if (flag)
                {
                    Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Persistir, usuario, null);
                    return Json(new { Status = true, Type = "success", Message = "Salvo com sucesso", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}