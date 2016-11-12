using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControleDocumentos.Models
{
    public class ChamadaModel
    {
        public List<ControleDocumentosLibrary.Aluno> Alunos { get; set; }
        public int NumChamada { get; set; }
        public int IdEvento { get; set; }
        public string NomeEvento { get; set; }
    }
}