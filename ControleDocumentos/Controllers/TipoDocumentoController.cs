using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleDocumentos.Controllers
{
    public class TipoDocumentoController : Controller
    {
        // GET: TipoDocumento
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CarregaModalCadastro(int? idTipo)
        {
            //instancia model
            if (idTipo.HasValue)
            {
                //pega model pelo id
            }
            //retorna model
            return PartialView("_CadastroTipoDocumento");
        }

        public ActionResult CarregaModalExclusao(int idTipo)
        {
            //get no tipo
            //retorna o tipo na partial
            return PartialView("_ExclusaoTipoDocumento");
        }
    }
}