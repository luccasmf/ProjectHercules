using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;

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
                                  select al).ToList();

            return alunos;
        }

        public Aluno GetAlunoById(int idAluno)
        {
            return db.Aluno.Find(idAluno);
        }
        
        public List<Aluno> GetAlunoByCursoId(int idCurso)
        {
            List<Aluno> alunos = (from al in db.Aluno
                                  join ac in db.AlunoCurso on al.IdAluno equals ac.IdAluno
                                  join c in db.Curso on ac.IdCurso equals c.IdCurso
                                  where c.IdCurso == idCurso
                                  select al
                                  ).ToList();

            return alunos;
        }
       
    }
}