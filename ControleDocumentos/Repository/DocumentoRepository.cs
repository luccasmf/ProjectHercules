using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;


namespace ControleDocumentos.Repository
{
    public class DocumentoRepository
    {
        DocumentosModel db = new DocumentosModel();
        AlunoRepository alunoRepository = new AlunoRepository();

        public Documento GetDocumentoByNome(string nome)
        {
            Documento doc = db.Documento.Where(d => d.NomeDocumento == nome).FirstOrDefault();

            return doc;
        }

        public Documento GetDocumentoById(int id)
        {
            return db.Documento.Find(id);
        }

        public List<Documento> GetAllDocs()
        {
            return db.Documento.ToList();
        }

        public bool PersisteDocumento(Documento doc)
        {
            Documento docOld = new Documento();
            if (!(doc.IdDocumento>0))
            {
                doc.AlunoCurso = db.AlunoCurso.Where(x => x.IdAluno == doc.AlunoCurso.IdAluno && x.IdCurso == doc.AlunoCurso.IdCurso).FirstOrDefault();
                db.Documento.Add(doc);
            }
            else
            {
                docOld = db.Documento.Find(doc.IdDocumento);
                docOld = Utilidades.ComparaValores(docOld, doc, new string[] { "NomeDocumento", "Data", "CaminhoDocumento" });
            }

            return db.SaveChanges() > 0;
        }

        public bool PersisteCertificados(List<Documento> docs)
        {
            foreach (Documento doc in docs)
                db.Documento.Add(doc);

            return db.SaveChanges() > 0;
        }

        public bool DeletaArquivo(Documento doc)
        {
            doc = db.Documento.Find(doc.IdDocumento);
            if (DirDoc.DeletaArquivo(doc.CaminhoDocumento))
            {
                db.Documento.Remove(doc);

                return db.SaveChanges() > 0;
            }
            return false;
        }
    }
}