using System;
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

        public List<Aluno> GetAlunosPresentes(Evento ev)
        {
            List<Aluno> alunos = (from a in db.Aluno
                                  join ae in db.AlunoEvento on a.IdAluno equals ae.IdAluno
                                  join e in db.Evento on ae.IdEvento equals e.IdEvento
                                  join p in db.Presenca on e.IdEvento equals p.IdEvento
                                  where ((e.IdEvento == ev.IdEvento) && (ae.Presenca >= e.PresencaNecessaria))
                                  select a).ToList();
            return alunos;

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

        /// <summary>
        /// Confirma presença para a lista de alunos
        /// </summary>
        /// <param name="idAluno">array com id dos alunos presentes</param>
        /// <param name="idEvento">id do evento</param>
        /// <returns>retorna um bool, dizendo se foi salvo ou não</returns>
        public bool AdicionaPresenca(int[] idAluno, int idEvento)
        {
            Evento ev = db.Evento.Find(idEvento);

            foreach (AlunoEvento ae in ev.AlunoEvento)
            {
                if (idAluno.Contains(ae.IdAluno))
                {
                    ae.Presenca++;
                    Presenca p = new Presenca();
                    p.IdAluno = ae.IdAluno;
                    p.IdEvento = idEvento;
                    p.Data = DateTime.Now;
                    ae.Presenca1.Add(p);
                }
            }

            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Pega relação de alunos inscritos no evento
        /// </summary>
        /// <param name="idEvento">id do evento desejado</param>
        /// <returns>lista de alunos inscritos no evento</returns>
        public List<Aluno> GetListaChamada(int idEvento)
        {
            List<Aluno> alunos = (from a in db.Aluno
                                  join ae in db.AlunoEvento on a.IdAluno equals ae.IdAluno
                                  where ae.IdEvento == idEvento
                                  select a).ToList();

            return alunos;
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