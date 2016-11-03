using ControleDocumentos.Filter;
using ControleDocumentos.Repository;
using ControleDocumentos.Util;
using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleDocumentos.Controllers
{
    //[AuthorizeAD(Groups = "G_FACULDADE_ALUNOS, G_FACULDADE_PROFESSOR_R, G_FACULDADE_PROFESSOR_RW, G_FACULDADE_COORDENADOR_R, G_FACULDADE_COORDENADOR_RW, G_FACULDADE_SECRETARIA_R, G_FACULDADE_SECRETARIA_RW")]
    public class SolicitacaoDocumentoAlunoController : BaseController
    {
        TipoDocumentoRepository tipoDocumentoRepository = new TipoDocumentoRepository();
        CursoRepository cursoRepository = new CursoRepository();
        AlunoRepository alunoRepository = new AlunoRepository();
        SolicitacaoDocumentoRepository solicitacaoRepository = new SolicitacaoDocumentoRepository();
        DocumentoRepository documentoRepository = new DocumentoRepository();

        // GET: SolicitacaoDocumentoAluno
        public ActionResult Index()
        {                      
            Utilidades.UsuarioLogado = new HomeController().GetSessionUser();
            //Aluno aluno = alunoRepository.GetAlunoByIdUsuario(Utilidades.UsuarioLogado.IdUsuario);
            //var id = aluno.IdAluno;

            var id = 1;
            return View(solicitacaoRepository.GetAguardandoAtendimentoAluno(id));
        }

        public ActionResult CarregarSolicitacao(int? idSol)
        {
            SolicitacaoDocumento sol = new SolicitacaoDocumento();

            if (idSol.HasValue)
            {
                sol = solicitacaoRepository.GetSolicitacaoById((int)idSol);

                // marca a solicitação como visualizada
                if (sol.Status == EnumStatusSolicitacao.pendente)
                {
                    sol.Status = EnumStatusSolicitacao.visualizado;
                    solicitacaoRepository.PersisteSolicitacao(sol);
                }
            }

            return PartialView("_VisualizacaoSolicitacao", sol);
        }

        public ActionResult List(bool apenasPendentes)
        {
            
            //Aluno aluno = alunoRepository.GetAlunoByIdUsuario(Utilidades.UsuarioLogado.IdUsuario);
            //var id = aluno.IdAluno;

            var id = 1;
            if (!apenasPendentes)
            {
                return PartialView("_List", solicitacaoRepository.GetTodosAluno(id));
            }
            else {
                return PartialView("_List", solicitacaoRepository.GetAguardandoAtendimentoAluno(id));
            }
        }

        #region Métodos auxiliares

        public object EnviarDocumento(SolicitacaoDocumento sol, HttpPostedFileBase uploadFile)
        {
            try
            {
                if (uploadFile == null)
                    return Json(new { Status = false, Type = "error", Message = "Selecione um documento" }, JsonRequestBehavior.AllowGet);


                var solicitacao = solicitacaoRepository.GetSolicitacaoById(sol.IdSolicitacao);
                solicitacao.Status = EnumStatusSolicitacao.processando;
                solicitacao.DataAtendimento = DateTime.Now;

                solicitacao.Documento.arquivo = converterFileToArray(uploadFile);
                solicitacao.Documento.NomeDocumento = uploadFile.FileName;

                string msgDoc = DirDoc.SalvaArquivo(solicitacao.Documento);
                switch (msgDoc)
                {
                    case "Falha ao persistir":
                        return Json(new { Status = false, Type = "error", Message = msgDoc }, JsonRequestBehavior.AllowGet);
                    case "Falha ao criptografar":
                        return Json(new { Status = false, Type = "error", Message = msgDoc }, JsonRequestBehavior.AllowGet);
                    default:
                        string msg = solicitacaoRepository.PersisteSolicitacao(solicitacao);

                        if (msg != "Erro")
                        {
                            try
                            {
                                var url = System.Web.Hosting.HostingEnvironment.MapPath("~/Views/Email/SolicitacaoDocumentoAtendida.cshtml");
                                string viewCode = System.IO.File.ReadAllText(url);

                                var solicitacaoEmail = solicitacaoRepository.ConverToEmailModel(sol, Url.Action("Login", "Account", null, Request.Url.Scheme));
                                var html = RazorEngine.Razor.Parse(viewCode, solicitacaoEmail);
                                // TODO talvez aqui tenha que buscar todos os usuarios tipo secretaria e enviar o email a todos
                                var to = new[] { solicitacaoEmail.EmailFuncionario };
                                var from = System.Configuration.ConfigurationManager.AppSettings["MailFrom"].ToString();
                                Email.EnviarEmail(from, to,string.Format("Solicitação de documento atendida - {0} - {1}", solicitacaoEmail.NomeTipoDocumento, solicitacaoEmail.NomeAluno), html);
                            }
                            catch (Exception)
                            {
                            }
                            Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Persistir, solicitacao, solicitacao.IdSolicitacao);
                            return Json(new { Status = true, Type = "success", Message = "Solicitação salva com sucesso", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Baixa arquivo
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>retorna o arquivo pra download</returns>
        public FileResult Download(string nomeDoc)
        {
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

        #endregion
    }
}