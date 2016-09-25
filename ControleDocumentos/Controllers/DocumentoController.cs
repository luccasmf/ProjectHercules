using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentosLibrary;
using System.IO;
using ControleDocumentos.Repository;

namespace ControleDocumentos.Controllers
{
    public class DocumentoController : Controller
    {
        TipoDocumentoRepository tipoDocumentoRepository = new TipoDocumentoRepository();

        // GET: Documento
        public ActionResult Index()
        {
            // apenas se decidirmos n usar o datatables como filtro
            // PopularDropDowns();

            return View(new List<Documento>());
        }

        public ActionResult CadastrarDocumento(int? idDoc)
        {
            PopularDropDowns();
            ViewBag.Alunos = new SelectList(new List<SelectListItem>() {
                new SelectListItem() {Text="Selecione um curso", Value="0"}
            }, "Value", "Text");

            //instancia model
            Documento doc = new Documento();

            if (idDoc.HasValue)
            {
                //pega model pelo id
            }
            //retorna model
            return View("CadastroDocumento", doc);
        }

        public ActionResult List(Models.DocumentoModel filter)
        {
            // apenas caso decidirmos n usar o datatables como filtro

            var retorno = new List<Documento>();
            //busca os documentos com base no filtro

            return PartialView("_List", retorno);
        }

        #region Métodos auxiliares

        private void PopularDropDowns()
        {
            //get todos os cursos
            var listCursos = new List<Curso>().Select(item => new SelectListItem
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
            ViewBag.TiposDoc = new SelectList(listTiposDoc, "Value", "Text");

        }

        public object SalvarDocumento(Documento doc, HttpPostedFileBase File) //da pra negociarmos esse parametro
        {
            try
            {
                doc.arquivo = converterFileToArray(File);
                string mensagem = DirDoc.SalvaArquivo(doc);

                switch (mensagem)
                {
                    case "Sucesso":
                        return Json(new { Status = true, Type = "success", Message = mensagem, ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                    case "Falha ao persistir":
                        return Json(new { Status = false, Type = "error", Message = mensagem }, JsonRequestBehavior.AllowGet);
                    case "Falha ao criptografar":
                        return Json(new { Status = false, Type = "error", Message = mensagem }, JsonRequestBehavior.AllowGet);
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação" }, JsonRequestBehavior.AllowGet);
            }
            
        }

        /// <summary>
        /// Baixa arquivo
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>retorna o arquivo pra download</returns>
        /// 
        public FileResult Download(Documento doc) //da pra vermos o melhor parametro
        {
            string nomeArquivo = doc.NomeDocumento;
            string extensao = Path.GetExtension(nomeArquivo);

            string contentType = "application/" + extensao.Substring(1);

            byte[] bytes = DirDoc.BaixaArquivo(doc.NomeDocumento);

            return File(bytes, contentType, nomeArquivo);

        }

        public static byte[] converterFileToArray(HttpPostedFileBase x)
        {
            MemoryStream tg = new MemoryStream();
            x.InputStream.CopyTo(tg);
            byte[] data = tg.ToArray();

            return data;
        }

        public JsonResult GetAlunosByIdCurso(int idCurso) {
            //get alunos por curso
            var lstAlunos = new List<Aluno>();
            return Json(lstAlunos.Select(x => new { Value = x.IdAluno, Text = x.Usuario.Nome }));
        }
        #endregion
    }
}