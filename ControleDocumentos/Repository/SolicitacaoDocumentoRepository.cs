using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;


namespace ControleDocumentos.Repository
{
    public class SolicitacaoDocumentoRepository
    {
        DocumentosModel db = new DocumentosModel();

        public SolicitacaoDocumento GetSolicitacaoById(int id)
        {
            return db.SolicitacaoDocumento.Find(id);
        }

        public List<SolicitacaoDocumento> GetAll()
        {
            return db.SolicitacaoDocumento.ToList();
        }

        public bool Salvar(SolicitacaoDocumento sol)
        {
            return true;
        }

        public bool DeletaArquivo(SolicitacaoDocumento sol)
        {
            return true;
        }
    }
}