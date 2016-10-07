using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;
using System.Reflection;
using ControleDocumentos.Util;

namespace ControleDocumentos.Repository
{
    public class CursoRepository
    {
        DocumentosModel db = new DocumentosModel();

        public List<Curso> GetCursos()
        {
            return db.Curso.ToList();
        }

        public Curso GetCursoById(int idCurso)
        {
            return db.Curso.Find(idCurso);
        }

        public List<Curso> GetCursosByAlunoId(int idAluno)
        {
            List<Curso> cursos = (from c in db.Curso
                                  join ac in db.AlunoCurso on c.IdCurso equals ac.IdCurso
                                  where ac.IdAluno == idAluno
                                  select c).ToList();

            return cursos;
        }

        public string PersisteCurso(Curso curso)
        {
            if (curso.IdCurso > 0)
            {
                return ComparaInfos(curso);
            }
            else
            {
                db.Curso.Add(curso);

            }
            if (db.SaveChanges() > 0)
            {
                return "Cadastrado";
            }
            else
            {
                return "Erro";
            }
        }

        public List<Curso> GetCursosByEventoId(int idEvento)
        {
            List<Curso> cursos = (from evento in db.Evento
                                  join curso in db.Curso on (evento.Curso.Select(x => x.IdCurso).FirstOrDefault()) equals curso.IdCurso
                                  where evento.IdEvento == idEvento
                                  select curso).ToList();

            return cursos;
        }

        public bool MatriculaAluno(int idCurso, int idAluno)
        {
            AlunoCurso matricula = new AlunoCurso();
            Curso c = db.Curso.Find(idCurso);

            matricula.IdAluno = idAluno;
            matricula.IdCurso = idCurso;
            matricula.HoraNecessaria = c.HoraComplementar;
            matricula.HoraCompleta = 0;

            db.AlunoCurso.Add(matricula);

            return db.SaveChanges() > 0;
        }

        public bool AtribuiHoras(int idAluno, int idCurso, int hora)
        {
            AlunoCurso ac = db.AlunoCurso.Find(idAluno, idCurso);
            ac.HoraCompleta += hora;

            return db.SaveChanges() > 0;
        }

        private string ComparaInfos(Curso c)
        {
            Curso cursoOld = db.Curso.Find(c.IdCurso);

            cursoOld = Utilidades.ComparaValores(cursoOld, c, new string[] { "Nome", "Nível", "HoraComplementar", "IdCoordenador" });

            if (db.SaveChanges() > 0)
            {
                return "Alterado";
            }
            else
            {
                return "Erro";
            }
        }
    }
}