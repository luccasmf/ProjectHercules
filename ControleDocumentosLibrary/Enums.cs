using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleDocumentosLibrary
{
    public enum EnumPermissaoUsuario :int
    {
        [Description("Aluno")]
        aluno=1,
        [Description("Professor")]
        professor,
        [Description("Coordenador")]
        coordenador,
        [Description("Secretaria")]
        secretaria,
    }

    public enum EnumStatusSolicitacao : int
    {
        [Description("Pendente")]
        pendente = 1,
        [Description("Visualizado")]
        visualizado,
        [Description("Processando")]
        processando,
        [Description("Concluído")]
        concluido,
        [Description("Cancelado")]
        cancelado,
    }

    public enum EnumStatusEvento : int
    {
        [Description("Aprovado")]
        aprovado = 1,
        [Description("Cancelado")]
        cancelado,
        [Description("Adiado")]
        adiado,
        [Description("Concluído")]
        concluido,

    }

    public enum EnumSession : int
    {
        Usuario
    }

    public enum EnumTipoSolicitacao: int
    {
        [Description("Solicitação do aluno")]
        aluno=1,

        [Description("Solicitação da secretaria")]
        secretaria
    }

}
