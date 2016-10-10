using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;


namespace ControleDocumentos.Repository
{
    public class DocumentoRepository
    {
        DocumentosModel db = new DocumentosModel();

        public Documento GetDocumentoByNome(string nome)
        {
            Documento doc = db.Documento.Where(d => d.NomeDocumento == nome).FirstOrDefault();

            return doc;
        }

        public List<Documento> GetAllDocs()
        {
            return db.Documento.ToList();
        }

        public bool PersisteDocumento(Documento doc)
        {
            db.Documento.Add(doc);

            return db.SaveChanges() > 0;
        }

        public bool DeletaArquivo(Documento doc)
        {
            doc = db.Documento.Find(doc.IdDocumento);
            if(DirDoc.DeletaArquivo(doc.CaminhoDocumento))
            {
                db.Documento.Remove(doc);

                return db.SaveChanges() > 0;
            }
            return false;
        }
    }
}