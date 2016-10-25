using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;
using System.IO;

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

        public string PersisteSolicitacao(SolicitacaoDocumento sol)
        {
            if (sol.IdSolicitacao > 0)
            {
                return ComparaInfos(sol);
            }
            else
            {
                db.SolicitacaoDocumento.Add(sol);

            }
            if (db.SaveChanges() > 0)
            {
                return "Cadastrado";
            }
            else
            {
                return "Erro";
            }
        }

        public bool DeletaArquivo(SolicitacaoDocumento sol)
        {
            SolicitacaoDocumento sd = db.SolicitacaoDocumento.Find(sol.IdSolicitacao);
            File.Delete(sd.Documento.CaminhoDocumento);
            sd.Documento.CaminhoDocumento = null;

            return db.SaveChanges() > 0;
        }

        private string ComparaInfos(SolicitacaoDocumento sol)
        {
            SolicitacaoDocumento solicitacaoOld = db.SolicitacaoDocumento.Find(sol.IdSolicitacao);

            solicitacaoOld = Utilidades.ComparaValores(solicitacaoOld, sol, new string[] { "DataLimite", "DataAtendimento", "Status", "IdDocumento", "IdFuncionario", "Observacao" });

            if (db.SaveChanges() > 0)
            {
                return "Alterado";
            }
            else
            {
                return "Erro";
            }
        }
    }
}