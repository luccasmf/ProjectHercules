using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleDocumentosLibrary
{
    public enum EnumStatusSolicitacao : int
    {
        pendente =1,
        visualizado,
        processando,
        concluido,
        cancelado,
    }
}
