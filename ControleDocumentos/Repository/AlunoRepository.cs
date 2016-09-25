using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;

namespace ControleDocumentos.Repository
{
    public class AlunoRepository
    {
        DocumentosModel db = new DocumentosModel();

        public List<Aluno> GetAlunoByIdCurso(int idCurso)
        {
            List<Aluno> alunos = (from al in db.Aluno
                                  join ac in db.AlunoCurso on al.IdAluno equals ac.IdAluno
                                  join cu in db.Curso on ac.IdCurso equals cu.IdCurso
                                  where cu.IdCurso == idCurso
                                  select al
                                  ).ToList();

            return alunos;
        }
    }
}