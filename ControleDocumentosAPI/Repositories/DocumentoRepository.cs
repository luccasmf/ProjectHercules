using ControleDocumentosAPI.Models;
using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControleDocumentosAPI.Repositories
{
    /// <summary>
    /// repositório para manipulação de dados da entidade Documento
    /// </summary>
    public class DocumentoRepository
    {
        private DocumentosModel db = new DocumentosModel();
        
        /// <summary>
        /// retorna todos os documentos cadastrados
        /// </summary>
        /// <returns>IQueryable de documentos</returns>
        public IQueryable<Documento> GetDocumento()
        {
            return db.Documento;
        } 

        /// <summary>
        /// Salva as informações do documento recebido no banco de dados
        /// </summary>
        /// <param name="doc">documento a ser salvo</param>
        /// <returns>true or false (sucesso ou falha)</returns>
        public bool SalvaDocumento(Documento doc)
        {
            db.Documento.Add(doc);

            return db.SaveChanges() > 0;
        }
    }

}