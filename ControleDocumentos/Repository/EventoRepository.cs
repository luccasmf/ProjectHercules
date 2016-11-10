using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;
using ControleDocumentos.Util;
using ControleDocumentos.Models;

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

        public List<Evento> GetEventoByAlunoInscrito(int idAluno)
        {
            List<Evento> eventos = (from e in db.Evento
                                    join ae in db.AlunoEvento on e.IdEvento equals ae.IdEvento
                                    where ae.IdAluno == idAluno
                                    select e).ToList();

            return eventos;
        }

        public List<Evento> GetEventoDisponivelByCurso (int idCurso)
        {
            List<Evento> eventos = GetEventoByCurso(idCurso);

            eventos = eventos.Where(x => x.DataInicio > DateTime.Now && x.VagasPreenchidas < x.Vagas).ToList();

            return eventos;
        }

        public List<Evento> GetEventosByAluno(string idUsuario)
        {
            List<Evento> eventos = (from e in db.Evento
                                    join ae in db.AlunoEvento on e.IdEvento equals ae.IdEvento
                                    join a in db.Aluno on ae.IdAluno equals a.IdAluno
                                    where a.IdUsuario == idUsuario
                                    select e).ToList();

            return eventos;
        }

        public List<Evento> GetByFilter(EventoFilter filter)
        {
            List<Evento> eventos = new List<Evento>();

            if (!string.IsNullOrEmpty(filter.NomeEvento) && filter.IdStatus == null)
            {
                eventos = db.Evento.Where(x => x.NomeEvento.Contains(filter.NomeEvento)).ToList();
            }
            else if (filter.IdStatus!=null && string.IsNullOrEmpty(filter.NomeEvento))
            {
                eventos = db.Evento.Where(x => x.Status == (EnumStatusEvento)filter.IdStatus).ToList();
            }
            else if (filter.IdStatus!=null && !string.IsNullOrEmpty(filter.NomeEvento))
            {
                eventos = db.Evento.Where(x => x.Status == (EnumStatusEvento)filter.IdStatus && x.NomeEvento.Contains(filter.NomeEvento)).ToList();
            }
            else
            {
                eventos = db.Evento.ToList();
            }


            return eventos;
        }

        public List<Aluno> GetAlunosPresentes(Evento ev)
        {
            List<Aluno> alunos = (from a in db.Aluno
                                  join ae in db.AlunoEvento on a.IdAluno equals ae.IdAluno
                                  join e in db.Evento on ae.IdEvento equals e.IdEvento
                                  join p in db.Presenca on e.IdEvento equals p.IdEvento
                                  where ((e.IdEvento == ev.IdEvento) && (ae.QuantidadePresenca >= e.PresencaNecessaria))
                                  select a).ToList();
            return alunos;

        }

        public string PersisteEvento(Evento ev, int[] idsCurso)
        {
            if (ev.IdEvento > 0)
            {
                return ComparaInfos(ev, idsCurso);
            }
            else
            {
                List<Curso> cursos = db.Curso.Where(c => idsCurso.Contains(c.IdCurso)).ToList();

                foreach(var c in cursos)
                {
                    ev.Curso.Add(c);
                }
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
            Evento e = db.Evento.Find(idEvento);

            if(e.VagasPreenchidas>=e.Vagas)
            {
                return false;
            }
            AlunoEvento ae = new AlunoEvento();
            ae.IdAluno = idAluno;
            //ae.IdEvento = idEvento;
            ae.QuantidadePresenca = 0;            
            e.AlunoEvento.Add(ae);
            e.VagasPreenchidas++;

            return db.SaveChanges() > 0;
        }

        public bool DesinscreverAluno(int idAluno, int idEvento)
        {
            Evento e = db.Evento.Find(idEvento);
            AlunoEvento ae = db.AlunoEvento.Where(x => x.IdAluno == idAluno && x.IdEvento == idEvento).FirstOrDefault();
            e.AlunoEvento.Remove(ae);
            e.VagasPreenchidas--;

            return db.SaveChanges()>0;
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
                    ae.QuantidadePresenca++;
                    Presenca p = new Presenca();
                    p.IdAluno = ae.IdAluno;
                    p.IdEvento = idEvento;
                    p.Data = DateTime.Now;
                    ae.Presenca.Add(p);
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

        private string ComparaInfos(Evento ev, int[] idsCurso)
        {
            DocumentosModel db2 = new DocumentosModel();
            Evento eventoOld = db2.Evento.Find(ev.IdEvento);

            eventoOld = Utilidades.ComparaValores(eventoOld, ev, new string[] { "NomeEvento", "Vagas", "VagasPreenchidas", "CargaHoraria", "PresencaNecessaria", "DataInicio", "DataFim", "Status", "Local", "Observacao" });

            if (eventoOld == null)
            {
                if (idsCurso == null)
                {
                    return "Mantido";
                }
                else
                {
                    List<Curso> cursos = db2.Curso.Where(c => idsCurso.Contains(c.IdCurso)).ToList();

                    eventoOld = db2.Evento.Find(ev.IdEvento);
                    eventoOld.Curso.Clear();
                    foreach (var c in cursos)
                    {

                        eventoOld.Curso.Add(c);
                    }
                }
            }

            try
            {
                if (db2.SaveChanges() > 0)
                {
                    return "Alterado";
                }
                else
                {
                    return "Mantido";
                }
            }
            catch
            {
                return "Erro";
            }
            
        }
    }
}
