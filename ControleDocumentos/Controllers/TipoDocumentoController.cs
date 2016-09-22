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
    }
}