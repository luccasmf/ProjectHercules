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

        public List<SolicitacaoDocumento> GetAguardandoAtendimentoAluno(int idUsuario)
        {
            List<SolicitacaoDocumento> solicitacoesDocumento = new List<SolicitacaoDocumento>();

            solicitacoesDocumento = db.SolicitacaoDocumento.Where(x => x.AlunoCurso.Aluno.IdAluno == idUsuario &&
           (x.Status == EnumStatusSolicitacao.pendente ||
           x.Status == EnumStatusSolicitacao.visualizado)).ToList();

            return solicitacoesDocumento;
        }

        public List<SolicitacaoDocumento> GetTodosAluno(int idAluno)
        {
            List<SolicitacaoDocumento> solicitacoesDocumento = new List<SolicitacaoDocumento>();
            solicitacoesDocumento = db.SolicitacaoDocumento.Where(x => x.AlunoCurso.IdAluno == idAluno).ToList();

            return solicitacoesDocumento;
        }

        public List<SolicitacaoDocumento> GetByFilter(SolicitacaoDocumentoFilter filter)
        {
            List<SolicitacaoDocumento> solicitacosDocumento = new List<SolicitacaoDocumento>();

            if (filter.IdCurso != null && filter.IdStatus != null) //se nenhum for nulo faz esse
            {
                solicitacosDocumento = solicitacosDocumento.Concat(db.SolicitacaoDocumento.Where(x => x.AlunoCurso.IdCurso == filter.IdCurso && x.Status == (EnumStatusSolicitacao)filter.IdStatus).ToList()).ToList();

            }
            else if (filter.IdCurso != null) //se status for nulo faz esse
            {
                solicitacosDocumento = solicitacosDocumento.Concat(db.SolicitacaoDocumento.Where(x => x.AlunoCurso.IdCurso == filter.IdCurso).ToList()).ToList();
            }
            else if (filter.IdStatus != null) //se curso for nulo faz esse
            {
                solicitacosDocumento = solicitacosDocumento.Concat(db.SolicitacaoDocumento.Where(x => x.Status == (EnumStatusSolicitacao)filter.IdStatus).ToList()).ToList();
            }
            else //se ambos forem nulos faz esse
            {
                solicitacosDocumento = solicitacosDocumento.Concat(db.SolicitacaoDocumento.ToList()).ToList();
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

        #region alterarStatus: método pra caso de erro do outro
        //public string AlteraStatus(SolicitacaoDocumento sol, EnumStatusSolicitacao e)
        //{
        //    SolicitacaoDocumento soli = db.SolicitacaoDocumento.Find(sol.IdSolicitacao);
        //    soli.Status = e;

        //    if(db.SaveChanges()>0)
        //    {
        //        return "Sucesso";
        //    }
        //    else
        //    {
        //        return "Erro";
        //    }
        //}
        #endregion

        private string ComparaInfos(SolicitacaoDocumento sol)
        {
            DocumentosModel db2 = new DocumentosModel();
            SolicitacaoDocumento solicitacaoOld = (from sd in db2.SolicitacaoDocumento where sd.IdSolicitacao == sol.IdSolicitacao select sd).FirstOrDefault();
            //db.Entry(solicitacaoOld).Reload();
            solicitacaoOld = Utilidades.ComparaValores(solicitacaoOld, sol, new string[] { "DataLimite", "DataAtendimento", "Status", "IdFuncionario", "Observacao" });

            if (solicitacaoOld == null)
            {
                return "Mantido";
            }
            if (db2.SaveChanges() > 0)
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