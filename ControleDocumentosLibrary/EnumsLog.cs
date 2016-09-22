using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleDocumentosLibrary
{
    public enum EnumAcao : int
    {
        Criar =1,
        Editar,
        Ativar,
        Desativar,
        Excluir,
    }

    public enum EnumTipoObjeto : int
    {
        Aluno=1,
        Curso,
        Documento,
        Evento,
        Funcionario,
        Usuario
    }
}
