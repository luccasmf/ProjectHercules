using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentosLibrary;
using System.IO;
using ControleDocumentos.Repository;
using ControleDocumentos.Filter;
using ControleDocumentos.Util.Extension;
using ControleDocumentos.Util;

namespace ControleDocumentos.Controllers
{
    //[AuthorizeAD(Groups = "G_FACULDADE_ALUNOS, G_FACULDADE_PROFESSOR_R, G_FACULDADE_PROFESSOR_RW, G_FACULDADE_COORDENADOR_R, G_FACULDADE_COORDENADOR_RW, G_FACULDADE_SECRETARIA_R, G_FACULDADE_SECRETARIA_RW")]
    public class DocumentoController : BaseController
    {
        TipoDocumentoRepository tipoDocumentoRepository = new TipoDocumentoRepository();
        CursoRepository cursoRepository = new CursoRepository();
        AlunoRepository alunoRepository = new AlunoRepository();
        DocumentoRepository documentoRepository = new DocumentoRepository();


        // GET: Documento
        public ActionResult Index()
        {
            // apenas se decidirmos n usar o datatables como filtro
            // PopularDropDowns();

            if(Utilidades.UsuarioLogado.Permissao == EnumPermissaoUsuario.coordenador)
            {
                List<Documento> retorno = new List<Documento>();
                // lucciros: Implementar metodo que busque apenas os docs vinculados a alunos que coordena
                //retorno = documentoRepository.GetDocumentosByIdCoordenador(Utilidades.UsuarioLogado.IdUsuario);
                return View(retorno);
            }
            return View(documentoRepository.GetAllDocs());
        }

        public ActionResult CadastrarDocumento(int? idDoc)
        {
            PopularDropDowns();
            Documento doc = new Documento();

            if (idDoc.HasValue)
            {
                doc = documentoRepository.GetDocumentoById((int)idDoc);
                PopularDropDownAlunos(doc.AlunoCurso.Curso.IdCurso);
            }
            else
            {
                ViewBag.Alunos = new SelectList(new List<SelectListItem>() { new SelectListItem() {
                    Text ="Selecione um curso",
                    Value =""}
                }, "Value", "Text");
            }
            return PartialView("_CadastroDocumento", doc);
        }

        public ActionResult List()
        {
            // helenira : replicar validação da index
            return PartialView("_List", documentoRepository.GetAllDocs());
            
        }

        public ActionResult CarregaModalExclusao(int idDoc)
        {
            Documento doc = documentoRepository.GetDocumentoById(idDoc);
            return PartialView("_ExclusaoDocumento", doc);
        }

        #region Métodos auxiliares

        private void PopularDropDowns()
        {
            List<Curso> lstCursos = new List<Curso>();
           if (Utilidades.UsuarioLogado.Permissao == EnumPermissaoUsuario.coordenador)
            {
                List<Documento> retorno = new List<Documento>();
                // lucciros : Implementar metodo que busque apenas os cursos do coordenador
                //lstCursos = pirulitinho;
            }
            else{
                lstCursos = cursoRepository.GetCursos();
            }

            var listCursosSelectList = lstCursos.Select(item => new SelectListItem
            {
                Value = item.IdCurso.ToString(),
                Text = item.Nome.ToString(),
            });
            ViewBag.Cursos = new SelectList(listCursosSelectList, "Value", "Text");


            var listTiposDoc = tipoDocumentoRepository.listaTipos().Select(item => new SelectListItem
            {
                Value = item.IdTipoDoc.ToString(),
                Text = item.TipoDocumento1.ToString(),
            });
            ViewBag.TiposDoc = new SelectList(listTiposDoc, "Value", "Text");

        }

        private void PopularDropDownAlunos(int idCurso) {
            // helenira: validar se o id do curso está entre os cursos do coordenador
            // get todos alunos pelo id do curso
            var listAlunos = alunoRepository.GetAlunoByIdCurso(idCurso).Select(item => new SelectListItem
            {
                Value = item.IdAluno.ToString(),
                Text = item.Usuario.Nome.ToString(),
            });
            ViewBag.Alunos = new SelectList(listAlunos, "Value", "Text");
        }

        public object SalvarDocumento(Documento doc, HttpPostedFileBase uploadFile)
        {
            // helenira : validar se o aluno é de algum curso do coordenador
            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadFile == null)
                        return Json(new { Status = false, Type = "error", Message = "Selecione um documento" }, JsonRequestBehavior.AllowGet);

                    if(!ValidaArquivo(uploadFile.FileName))
                    {
                        return Json(new { Status = false, Type = "error", Message = "Formato inválido" }, JsonRequestBehavior.AllowGet);

                    }

                    doc.arquivo = converterFileToArray(uploadFile);
                    doc.NomeDocumento = uploadFile.FileName;
                    string mensagem = DirDoc.SalvaArquivo(doc);

                    switch (mensagem)
                    {
                        case "Arquivo existente":
                            return Json(new { Status = true, Type = "success", Message = "Documento salvo com sucesso", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                        case "Sucesso":
                            doc.arquivo = null;
                            Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Persistir, doc, (doc.IdDocumento > 0 ? (int?)doc.IdDocumento:null));
                            return Json(new { Status = true, Type = "success", Message = "Documento salvo com sucesso", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                        case "Falha ao persistir":
                            return Json(new { Status = false, Type = "error", Message = mensagem }, JsonRequestBehavior.AllowGet);
                        case "Falha ao criptografar":
                            return Json(new { Status = false, Type = "error", Message = mensagem }, JsonRequestBehavior.AllowGet);
                        default:
                            return null;
                    }
                }
                catch (Exception e)
                {
                    return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação" }, JsonRequestBehavior.AllowGet);
                }
            }
            else {
                return Json(new { Status = false, Type = "error", Message = "Campos inválidos" }, JsonRequestBehavior.AllowGet);
            }
        }

        public object ExcluirDocumento(Documento doc)
        {
            // helenira: validar se aluno está ok qndo for coodenador
            doc = documentoRepository.GetDocumentoById(doc.IdDocumento);
            if (documentoRepository.DeletaArquivo(doc))
            {
                Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Excluir, doc, (int?)doc.IdDocumento);
                return Json(new { Status = true, Type = "success", Message = "Documento deletado com sucesso!", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Baixa arquivo
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>retorna o arquivo pra download</returns>
        /// 
        public FileResult Download(string nomeDoc)
        {
            // helenira, validar permissão de download caso seja coordenador

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
            // helenira: validar curso caso seja coordenador
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