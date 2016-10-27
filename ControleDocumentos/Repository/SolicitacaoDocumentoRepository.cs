using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;
using System.IO;
using System.Web.Script.Serialization;
using ControleDocumentos.Models;

namespace ControleDocumentos.Repository
{
    public class SolicitacaoDocumentoRepository
    {
        DocumentosModel db = new DocumentosModel();
        LogsRepository logsRepository = new LogsRepository();

        public SolicitacaoDocumento GetSolicitacaoById(int id)
        {
            return db.SolicitacaoDocumento.Find(id);
        }

        public List<SolicitacaoDocumento> GetAll()
        {
            return db.SolicitacaoDocumento.ToList();
        }

        public string PersisteSolicitacao(SolicitacaoDocumento sol, string idUsuario)
        {
            Logs log = new Logs();
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
                log.IdUsuario = idUsuario;
                log.Data = DateTime.Now;
                log.Acao = EnumAcao.Persistir;
                log.TipoObjeto = EnumTipoObjeto.SolicitacaoDocumento;
                log.IdObjeto = sol.IdSolicitacao;
                log.EstadoAnterior = new JavaScriptSerializer().Serialize(sol);

                if (logsRepository.SalvarLog(log))
                    return "Cadastrado";
                else
                    return "ErroLog";
            }
            else
            {
                return "Erro";
            }
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

        public List<SolicitacaoDocumento> GetByFilter(SolicitacaoDocumentoFilter filter)
        {
            List<SolicitacaoDocumento> solicitacosDocumento;

            if(filter.IdStatus == null)
            {
                solicitacosDocumento = db.SolicitacaoDocumento.Where(x => x.AlunoCurso.IdCurso == filter.IdCurso).ToList();
            }
            else if (filter.IdCurso == null)
            {
                solicitacosDocumento = db.SolicitacaoDocumento.Where(x => x.Status == (EnumStatusSolicitacao)filter.IdStatus).ToList();
            }
            else
            {
                solicitacosDocumento = db.SolicitacaoDocumento.Where(x=> x.AlunoCurso.IdCurso == filter.IdCurso && x.Status == (EnumStatusSolicitacao)filter.IdStatus).ToList();
            }
            
            return solicitacosDocumento;
        }

        public bool DeletaArquivo(SolicitacaoDocumento sol)
        {
            if (sol.Documento != null && sol.Documento.CaminhoDocumento != null)
            {
                File.Delete(sol.Documento.CaminhoDocumento);
                sol.Documento.CaminhoDocumento = null;
            }
            db.SolicitacaoDocumento.Remove(sol);
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