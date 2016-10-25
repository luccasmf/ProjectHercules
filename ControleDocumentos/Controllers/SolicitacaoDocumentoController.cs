using ControleDocumentos.Repository;
using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleDocumentos.Controllers
{
    public class SolicitacaoDocumentoController : Controller
    {
        TipoDocumentoRepository tipoDocumentoRepository = new TipoDocumentoRepository();
        CursoRepository cursoRepository = new CursoRepository();
        AlunoRepository alunoRepository = new AlunoRepository();
        SolicitacaoDocumentoRepository solicitacaoRepository = new SolicitacaoDocumentoRepository();
        DocumentoRepository documentoRepository = new DocumentoRepository();

        // GET: SolicitacaoDocumento
        public ActionResult Index()
        {
            PopularDropDowns();

            return View(solicitacaoRepository.GetAll());
        }

        public ActionResult CadastrarSolicitacao(int? idSol)
        {
            PopularDropDownsCadastro();
            SolicitacaoDocumento sol = new SolicitacaoDocumento();

            if (idSol.HasValue)
            {
                sol = solicitacaoRepository.GetSolicitacaoById((int)idSol);
                PopularDropDownAlunos(sol.AlunoCurso.Curso.IdCurso);
            }
            else
            {
                ViewBag.Alunos = new SelectList(new List<SelectListItem>() { new SelectListItem() {
                    Text ="Selecione um curso",
                    Value =""}
                }, "Value", "Text");
            }
            //retorna model
            return PartialView("_CadastroSolicitacao", sol);
        }

        public ActionResult List(Models.SolicitacaoDocumentoFilter filter)
        {
            //dps nos ve esse esquema de filtro loko aqui
            return PartialView("_List", solicitacaoRepository.GetAll());
        }

        public ActionResult CarregaModalExclusao(int idSol)
        {
            SolicitacaoDocumento sol = solicitacaoRepository.GetSolicitacaoById(idSol);
            return PartialView("_ExclusaoSolicitacao", sol);
        }

        #region Métodos auxiliares

        private void PopularDropDowns()
        {
            //get todos os cursos
            var listCursos = cursoRepository.GetCursos().Select(item => new SelectListItem
            {
                Value = item.IdCurso.ToString(),
                Text = item.Nome.ToString(),
            });
            ViewBag.Cursos = new SelectList(listCursos, "Value", "Text");


            var listStatus = Enum.GetValues(typeof(EnumStatusSolicitacao)).
                Cast<EnumStatusSolicitacao>().Select(v => new SelectListItem
                {
                    Text = Util.Extension.EnumExtensions.GetEnumDescription(v),
                    Value = ((int)v).ToString()
                }).ToList();
            ViewBag.Status = new SelectList(listStatus, "Value", "Text");
        }

        private void PopularDropDownsCadastro()
        {
            var listCursos = cursoRepository.GetCursos().Select(item => new SelectListItem
            {
                Value = item.IdCurso.ToString(),
                Text = item.Nome.ToString(),
            });
            ViewBag.Cursos = new SelectList(listCursos, "Value", "Text");

            var listTiposDoc = tipoDocumentoRepository.listaTipos().Select(item => new SelectListItem
            {
                Value = item.IdTipoDoc.ToString(),
                Text = item.TipoDocumento1.ToString(),
            });
            ViewBag.TiposDocumento = new SelectList(listTiposDoc, "Value", "Text");
        }

        private void PopularDropDownAlunos(int idCurso)
        {
            //get todos alunos pelo id do curso
            var listAlunos = alunoRepository.GetAlunoByIdCurso(idCurso).Select(item => new SelectListItem
            {
                Value = item.IdAluno.ToString(),
                Text = item.Usuario.Nome.ToString(),
            });
            ViewBag.Alunos = new SelectList(listAlunos, "Value", "Text");
        }

        public object SalvarSolicitacao(SolicitacaoDocumento sol)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // implementar esse metodo
                    string ok = solicitacaoRepository.PersisteSolicitacao(sol);

                    if(ok != "Erro")
                        return Json(new { Status = true, Type = "success", Message = "Documento salvo com sucesso", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
                }
            }
            else {
                return Json(new { Status = false, Type = "error", Message = "Campos inválidos" }, JsonRequestBehavior.AllowGet);
            }
        }

        public object ExcluirDocumento(SolicitacaoDocumento sol)
        {
            // implementar
            // adicionar regra que só deleta se tiver com status pendente
            // vou colocar a regra na view tbm mas é bom ter aqui tb

            if (sol.Status == EnumStatusSolicitacao.pendente) //regra q soh deleta se status for pendente
            {
                if (solicitacaoRepository.DeletaArquivo(sol))
                {
                    return Json(new { Status = true, Type = "success", Message = "Solicitação deletada com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                }
            }            
            return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Baixa arquivo
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>retorna o arquivo pra download</returns>
        public FileResult Download(string nomeDoc)
        {
            Documento doc = documentoRepository.GetDocumentoByNome(nomeDoc);

            string nomeArquivo = doc.NomeDocumento;
            string extensao = Path.GetExtension(nomeArquivo);

            string contentType = "application/" + extensao.Substring(1);

            byte[] bytes = DirDoc.BaixaArquivo(doc);

            return File(bytes, contentType, nomeArquivo);
        }

        public static byte[] converterFileToArray(HttpPostedFileBase x)
        {
            MemoryStream tg = new MemoryStream();
            x.InputStream.CopyTo(tg);
            byte[] data = tg.ToArray();

            return data;
        }

        public JsonResult GetAlunosByIdCurso(int idCurso)
        {
            if (idCurso > 0)
            {
                var lstAlunos = alunoRepository.GetAlunoByIdCurso(idCurso);
                return Json(lstAlunos.Select(x => new { Value = x.IdAluno, Text = x.Usuario.Nome }));
            }
            return Json(null);
        }
        #endregion
    }
}