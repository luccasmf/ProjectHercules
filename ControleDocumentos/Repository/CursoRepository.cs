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

        public string PersisteCurso(Curso curso)
        {
            if(curso.IdCurso>0)
            {
                return ComparaInfos(curso);
            }
            else
            {
                db.Curso.Add(curso);
               
            }
            if(db.SaveChanges() > 0)
            {
                return "Cadastrado";
            }
            else
            {
                return "Erro";
            }
        }

        private string ComparaInfos(Curso c)
        {
            Curso cursoOld = db.Curso.Find(c.IdCurso);

            cursoOld = Generics.ComparaValores(cursoOld, c, new string[] { "Nome", "Nível", "HoraComplementar", "IdCoordenador" });

            if(db.SaveChanges() >0)
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