﻿using System;
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
            if (!(doc.IdDocumento > 0))
            {
                doc.AlunoCurso = db.AlunoCurso.Where(x => x.IdAluno == doc.AlunoCurso.IdAluno && x.IdCurso == doc.AlunoCurso.IdCurso).FirstOrDefault();
                db.Documento.Add(doc);
            }
            else
            {
                docOld = db.Documento.Find(doc.IdDocumento);
                docOld = Utilidades.ComparaValores(docOld, doc, new string[] { "NomeDocumento", "Data", "CaminhoDocumento" });
            }

            if (docOld != null)
                return db.SaveChanges() > 0;
            return true;
        }

        public bool PersisteCertificados(List<Documento> docs)
        {
            foreach (Documento doc in docs)
                db.Documento.Add(doc);

            return db.SaveChanges() > 0;
        }

        public bool DeletaArquivo(Documento doc)
        {
            if (DirDoc.DeletaArquivo(doc.CaminhoDocumento))
            {
                db.Documento.Remove(doc);

                return db.SaveChanges() > 0;
            }
            return false;
        }

        public List<Documento> GetDocsByCursoId(int idCurso)
        {
            List<Documento> docs = (from doc in db.Documento
                                    join ac in db.AlunoCurso on doc.IdAlunoCurso equals ac.IdAlunoCurso
                                    join cur in db.Curso on ac.IdCurso equals cur.IdCurso
                                    where cur.IdCurso == idCurso
                                    select doc).ToList();

            //db.Documento.Where(x => x.AlunoCurso.Curso.IdCurso == idCurso).ToList();

            return docs;
        }

        public List<Documento> GetDocsByCoordenador(string idCoord)
        {
            List<Documento> docs = (from doc in db.Documento
                                    join ac in db.AlunoCurso on doc.IdAlunoCurso equals ac.IdAlunoCurso
                                    join cu in db.Curso on ac.IdCurso equals cu.IdCurso
                                    join fu in db.Funcionario on cu.IdCoordenador equals fu.IdFuncionario
                                    where fu.IdUsuario == idCoord
                                    select doc).ToList();

            return docs;
        }

        public List<Documento> GetDocsByAluno(string idAluno)
        {
            List<Documento> docs = (from doc in db.Documento
                                    join ac in db.AlunoCurso on doc.IdAlunoCurso equals ac.IdAlunoCurso
                                   join al in db.Aluno on ac.IdAluno equals al.IdAluno
                                    where al.IdUsuario == idAluno
                                    select doc).ToList();

            return docs;
        }
    }
}