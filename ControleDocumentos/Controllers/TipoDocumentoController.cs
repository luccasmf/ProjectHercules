using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleDocumentos.Repository;
using ControleDocumentosLibrary;

namespace ControleDocumentos.Controllers
{
    public class TipoDocumentoController : Controller
    {
        TipoDocumentoRepository tipoDocumentoRepository = new TipoDocumentoRepository();


        // GET: TipoDocumento
        public ActionResult Index()
        {
            return View(tipoDocumentoRepository.listaTipos());
        }

        public ActionResult CarregaModalCadastro(int? idTipo)
        {
            //instancia model
            TipoDocumento tipoDoc = new TipoDocumento();

            if (idTipo.HasValue)
            {
                //pega model pelo id
                tipoDoc = tipoDocumentoRepository.GetTipoDoc(idTipo);
            }
            //retorna model
            return PartialView("_CadastroTipoDocumento", tipoDoc);
        }

        public ActionResult CarregaModalExclusao(int idTipo)
        {
            //get no tipo
            TipoDocumento tipoDoc = tipoDocumentoRepository.GetTipoDoc(idTipo);

            //retorna o tipo na partial
            return PartialView("_ExclusaoTipoDocumento", tipoDoc);
        }

        public object SalvarTipoDocumento(TipoDocumento model)
        {
            try
            {
                if (model.IdTipoDoc > 0)
                {
                    //update
                }
                else {
                    //create
                }
                return Json(new { Status = true, Type = "success", Message = "Tipo de documento salvo com sucesso.", ReturnUrl = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Type = "error", Message = "Ocorreu um erro ao realizar esta operação" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult List()
        {
            return PartialView("_List", tipoDocumentoRepository.listaTipos());
        }
    }
}