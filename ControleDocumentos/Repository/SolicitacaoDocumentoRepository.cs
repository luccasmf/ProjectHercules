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
                try
                {
                    var url = System.Web.Hosting.HostingEnvironment.MapPath("~/Views/Email/NovaSolicitacaoDocumento.cshtml");
                    string viewCode = System.IO.File.ReadAllText(url);

                    var html = RazorEngine.Razor.Parse(viewCode, sol);
                    var to = new[] { sol.AlunoCurso.Aluno.Usuario.E_mail };
                    var from = System.Configuration.ConfigurationManager.AppSettings["MailFrom"].ToString();
                    Email.EnviarEmail(from, to, "Nova solicitação de documento - " + sol.Documento.TipoDocumento.TipoDocumento1, html);
                }
                catch (Exception)
                {
                }

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

        private string ComparaInfos(SolicitacaoDocumento sol)
        {
            SolicitacaoDocumento solicitacaoOld = db.SolicitacaoDocumento.Find(sol.IdSolicitacao);
            solicitacaoOld = Utilidades.ComparaValores(solicitacaoOld, sol, new string[] { "DataLimite", "DataAtendimento", "Status", "IdFuncionario", "Observacao" });

            if (solicitacaoOld == null)
            {
                return "Mantido";
            }
            if (db.SaveChanges() > 0)
            {
                try
                {
                    var url = System.Web.Hosting.HostingEnvironment.MapPath("~/Views/Email/AlteracaoSolicitacaoDocumento.cshtml");
                    string viewCode = System.IO.File.ReadAllText(url);

                    var html = RazorEngine.Razor.Parse(viewCode, sol);
                    var to = new[] { sol.AlunoCurso.Aluno.Usuario.E_mail };
                    var from = System.Configuration.ConfigurationManager.AppSettings["MailFrom"].ToString();
                    Email.EnviarEmail(from, to, "Alteração em solicitação de documento - " + sol.Documento.TipoDocumento.TipoDocumento1, html);
                }
                catch (Exception)
                {
                }

                return "Alterado";
            }
            else
            {
                return "Erro";
            }
        }
    }
}