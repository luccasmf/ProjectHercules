﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;

namespace ControleDocumentos.Repository
{
    public class EventoRepository
    {
        DocumentosModel db = new DocumentosModel();

        public List<Evento> GetEventos()
        {
            return db.Evento.ToList();
        }

        public Evento GetEventoById(int idEvento)
        {
            return db.Evento.Find(idEvento);
        }

        public List<Evento> GetEventoByCurso(int idCurso)
        {
            List<Evento> eventos = (from e in db.Evento
                                    join c in db.Curso on e.Curso.Select(ev => ev.IdCurso).FirstOrDefault() equals c.IdCurso
                                    where c.IdCurso == idCurso
                                    select e).ToList();

            return eventos;
        }

        public List<Evento> GetEventoByAluno(int idAluno)
        {
            List<Evento> eventos = (from e in db.Evento
                                    join ae in db.AlunoEvento on e.IdEvento equals ae.IdEvento
                                    where ae.IdAluno == idAluno
                                    select e).ToList();

            return eventos;
        }


        public string PersisteEvento(Evento ev)
        {
            if (ev.IdEvento > 0)
            {
                return ComparaInfos(ev);
            }
            else
            {
                db.Evento.Add(ev);

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

        public bool InscreveAluno(int idAluno, int idEvento)
        {
            AlunoEvento ae = new AlunoEvento();
            ae.IdAluno = idAluno;
            ae.IdEvento = idEvento;
            ae.Presenca = 0;

            db.AlunoEvento.Add(ae);

            return db.SaveChanges() > 0;
        }

        public bool AdicionaPresenca(int idAluno, int idEvento)
        {
            AlunoEvento ae = db.AlunoEvento.Find(idAluno, idEvento);
            ae.Presenca++;

            return db.SaveChanges() > 0;
        }

        private string ComparaInfos(Evento ev)
        {
            Evento eventoOld = db.Evento.Find(ev.IdEvento);

            eventoOld = Utilidades.ComparaValores(eventoOld, ev, new string[] { "NomeEvento", "Vagas", "VagasPreenchidas", "CargaHoraria", "PresencaNecessaria", "DataInicio", "DataFim", "Status", "Local", "Observacao" });

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