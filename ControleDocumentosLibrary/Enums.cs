using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleDocumentosLibrary
{
    public enum EnumPermissaoUsuario :int
    {
        aluno=1,
        professor,
        coordenador,
        secretaria,
    }

    public enum EnumStatusSolicitacao : int
    {
        pendente = 1,
        visualizado,
        processando,
        concluido,
        cancelado,
    }

    public enum EnumStatusEvento : int
    {
        aprovado = 1,
        cancelado,
        adiado,
        concluido,

    }

    public enum EnumTipoDocumento : int
    {
        certificado = 1,
        comprovanteDeEndereco,
    }
}
