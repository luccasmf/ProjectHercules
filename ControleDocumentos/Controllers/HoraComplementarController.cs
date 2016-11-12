using ControleDocumentos.Repository;
using ControleDocumentos.Util;
using ControleDocumentos.Util.Extension;
using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleDocumentos.Controllers
{
    public class HoraComplementarController : Controller
    {
        TipoDocumentoRepository tipoDocumentoRepository = new TipoDocumentoRepository();
        CursoRepository cursoRepository = new CursoRepository();
        AlunoRepository alunoRepository = new AlunoRepository();
        SolicitacaoDocumentoRepository solicitacaoRepository = new SolicitacaoDocumentoRepository();
        DocumentoRepository documentoRepository = new DocumentoRepository();

        // GET: HoraComplementar
        public ActionResult Index()
        {
            PopularDropDowns();

            // lucciros, pega essas infos aqui pfvr
            ViewBag.HrsComputadas = 20;
            ViewBag.HrsNecessarias = 400;

            // retornar as solicitações feitas pelo aluno com status "processando"
            //List<SolicitacaoDocumento> retorno = solicitacaoRepository.GetSolicitacaoByAluno(Utilidades.UsuarioLogado.IdUsuario).Where(x => x.Status == EnumStatusSolicitacao.processando).ToList();
            return View(new List<SolicitacaoDocumento>());
        }

        public ActionResult CadastrarSolicitacao(int? idSol)
        {
            SolicitacaoDocumento sol = new SolicitacaoDocumento();

            if (idSol.HasValue)
            {
                sol = solicitacaoRepository.GetSolicitacaoById((int)idSol);
            }
            return PartialView("_CadastroSolicitacao", sol);
        }

        public ActionResult List(Models.SolicitacaoDocumentoFilter filter)
        {
            // lucciros apenas as solicitações do aluno 
            // n esquece de filtrar por tipo de solicitação tb 
            return PartialView("_List", solicitacaoRepository.GetByFilter(filter));
        }

        public ActionResult CarregaModalExclusao(int idSol)
        {
            SolicitacaoDocumento sol = solicitacaoRepository.GetSolicitacaoById(idSol);
            return PartialView("_ExclusaoSolicitacao", sol);
        }

        public ActionResult CarregaModalConfirmacao(EnumStatusSolicitacao novoStatus, int idSol)
        {
            SolicitacaoDocumento sol = new SolicitacaoDocumento { IdSolicitacao = idSol, Status = novoStatus };
            return PartialView("_CancelamentoSolicitacao", sol);
        }

        public object SalvarSolicitacao(SolicitacaoDocumento sol, HttpPostedFileBase uploadFile)
        {
            if (uploadFile == null)
                return Json(new { Status = false, Type = "error", Message = "Selecione um documento" }, JsonRequestBehavior.AllowGet);

            try
            {
                var edit = true;
                sol.Status = sol.IdSolicitacao > 0 ? sol.Status : EnumStatusSolicitacao.pendente;
                sol.DataAbertura = DateTime.Now;

                 
                // lucciros tem que pegar o AlunoCurso do usuario logado sla como vc vai fazer isso heroorwhrwe
                // tem que colocar tbm o filtro por tipo de solicitação em solicitação documento e 
                // solicitação documento aluno pra n misturara as coisas
                //if (sol.IdSolicitacao == 0)
                //    sol.IdAlunoCurso = cursoRepository.GetAlunoCurso(sol.AlunoCurso.IdAluno, sol.AlunoCurso.IdCurso).IdAlunoCurso;

                sol.AlunoCurso = null;
                sol.TipoSolicitacao = EnumTipoSolicitacao.aluno;
                if (sol.IdSolicitacao == 0)
                {
                    edit = false;

                    sol.Documento = new Documento();
                    sol.Documento.arquivo = converterFileToArray(uploadFile);
                    sol.Documento.NomeDocumento = uploadFile.FileName;

                    string msgDoc = DirDoc.SalvaArquivo(sol.Documento);
                }

                string msg = solicitacaoRepository.PersisteSolicitacao(sol);

                if (msg != "Erro")
                {
                    //try
                    //{
                    //    DocumentosModel db2 = new DocumentosModel();
                    //    var solicitacao = db2.SolicitacaoDocumento.Find(sol.IdSolicitacao);
                    //    var solicitacaoEmail = solicitacaoRepository.ConverToEmailModel(solicitacao, Url.Action("Login", "Account", null, Request.Url.Scheme));

                    //    if (edit)
                    //    {
                    //        var url = System.Web.Hosting.HostingEnvironment.MapPath("~/Views/Email/AlteracaoSolicitacaoDocumento.cshtml");
                    //        string viewCode = System.IO.File.ReadAllText(url);

                    //        var html = RazorEngine.Razor.Parse(viewCode, solicitacaoEmail);
                    //        var to = new[] { solicitacaoEmail.EmailAluno };
                    //        var from = System.Configuration.ConfigurationManager.AppSettings["MailFrom"].ToString();
                    //        Email.EnviarEmail(from, to, "Alteração em solicitação de documento - " + solicitacaoEmail.NomeTipoDocumento, html);
                    //    }
                    //    else {
                    //        var url = System.Web.Hosting.HostingEnvironment.MapPath("~/Views/Email/NovaSolicitacaoDocumento.cshtml");
                    //        string viewCode = System.IO.File.ReadAllText(url);

                    //        var html = RazorEngine.Razor.Parse(viewCode, solicitacaoEmail);
                    //        var to = new[] { solicitacaoEmail.EmailAluno };
                    //        var from = System.Configuration.ConfigurationManager.AppSettings["MailFrom"].ToString();
                    //        Email.EnviarEmail(from, to, "Nova solicitação de documento - " + solicitacaoEmail.NomeTipoDocumento, html);
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //}

                    Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Persistir, sol, (sol.IdSolicitacao > 0 ? (int?)sol.IdSolicitacao : null));
                    return Json(new { Status = true, Type = "success", Message = "Solicitação salva com sucesso", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
            }
        }


        public object CancelarSolicitacao(SolicitacaoDocumento solic)
        {
            try
            {
                var sol = solicitacaoRepository.GetSolicitacaoById(solic.IdSolicitacao);
                sol.Status = EnumStatusSolicitacao.cancelado;
                if (sol.Status == EnumStatusSolicitacao.cancelado && !string.IsNullOrEmpty(sol.Documento.CaminhoDocumento))
                {
                    DirDoc.DeletaArquivo(sol.Documento.CaminhoDocumento);
                    sol.Documento.CaminhoDocumento = null;
                }
                string msg = solicitacaoRepository.PersisteSolicitacao(sol);

                if (msg != "Erro")
                {
                    Utilidades.SalvaLog(Utilidades.UsuarioLogado, EnumAcao.Cancelar, sol, sol.IdSolicitacao);

                    return Json(new { Status = true, Type = "success", Message = "Solicitação salva com sucesso", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Download(string nomeDoc)
        {
            Documento doc = documentoRepository.GetDocumentoByNome(nomeDoc);

            if (Utilidades.UsuarioLogado.Permissao == EnumPermissaoUsuario.coordenador)
            {
                List<Documento> retorno = documentoRepository.GetDocsByCoordenador(Utilidades.UsuarioLogado.IdUsuario);
                if (!retorno.Any(x => x.IdDocumento == doc.IdDocumento))
                    return RedirectToAction("Unauthorized", "Error");
            }

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

        private void PopularDropDowns()
        {
            var listStatus = Enum.GetValues(typeof(EnumStatusSolicitacao)).
                Cast<EnumStatusSolicitacao>().Select(v => new SelectListItem
                {
                    Text = EnumExtensions.GetEnumDescription(v),
                    Value = ((int)v).ToString(),
                }).ToList();
            ViewBag.Status = new SelectList(listStatus, "Value", "Text", ((int)EnumStatusSolicitacao.processando).ToString());
        }

    }
}