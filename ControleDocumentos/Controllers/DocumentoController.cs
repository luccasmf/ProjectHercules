using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentosLibrary;
using System.IO;

namespace ControleDocumentos.Controllers
{
    public class DocumentoController : Controller
    {
        // GET: Documento
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Baixa arquivo
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>retorna o arquivo pra download</returns>
        public FileResult Download (Documento doc) //da pra vermos o melhor parametro
        {
            string nomeArquivo = doc.NomeDocumento;
            string extensao = Path.GetExtension(nomeArquivo);

            string contentType = "application/"+extensao.Substring(1);

            byte[] bytes = DirDoc.BaixaArquivo(doc.NomeDocumento);

            return File(bytes, contentType, nomeArquivo);
            
        }



        public object Upload (Documento doc) //da pra negociarmos esse parametro
        {
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
    }
}