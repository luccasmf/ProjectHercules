using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ControleDocumentosAPI.Repositories;
using ControleDocumentosLibrary;
using ControleDocumentosAPI.Models;

namespace ControleDocumentosAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentoController : ApiController
    {
        private DocumentoRepository repoDoc = new DocumentoRepository();

        // GET: api/Documento
        /// <summary>
        /// Solicita todos os documentos
        /// </summary>
        /// <returns></returns>
        public IQueryable<Documento> GetDocumento()
        {
            return repoDoc.GetDocumento();
        }

        // POST: api/Documento/PostSalvaDocumento?doc=
        /// <summary>
        /// Salva o documento nos diretorios e cadastra no banco de dados
        /// </summary>
        /// <param name="doc">documento enviado pelo usuário</param>
        /// <returns>Ok ou NotFound</returns>
        public IHttpActionResult PostSalvaDocumento(Documento doc)
        {           
            Documento documento = DirDoc.SalvaArquivo(doc);
            documento.arquivo = null;

            if(repoDoc.SalvaDocumento(documento))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
