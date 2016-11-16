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
            return View(Utilidades.UsuarioLogado);
        }

        public ActionResult DadosCadastrais()
        {
            var listCursosSelectList = cursoRepository.GetCursos().Select(item => new SelectListItem
            {
                Value = item.IdCurso.ToString(),
                Text = item.Nome.ToString(),
            });
            ViewBag.Cursos = new SelectList(listCursosSelectList, "Value", "Text");
           
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