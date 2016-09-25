using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;

namespace ControleDocumentos.Repository
{
    public class CursoRepository
    {
        DocumentosModel db = new DocumentosModel();

        public List<Curso> GetCursos()
        {
            return db.Curso.ToList();
        }
    }
}