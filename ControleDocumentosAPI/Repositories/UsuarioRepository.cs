using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControleDocumentosAPI.Repositories
{
    /// <summary>
    /// repositório para acesso a dados da entidade de usuario
    /// </summary>
    public class UsuarioRepository
    {
        private DocumentosModel db = new DocumentosModel();

        public IQueryable<Usuario> GetUsuario()
        {
            return db.Usuario;
        }

        public Usuario GetUsuarioById(string id)
        {
            return db.Usuario.Find(id);
        }

        public Usuario DeleteUsuario(string id)
        {
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return null;
            }
            else
            {
                db.Usuario.Remove(usuario);
                db.SaveChanges();
                return usuario;
            }
        }

        public int SalvaUsuario(Usuario usuario)
        {
            try
            {
                db.Usuario.Add(usuario);
                if (usuario.Permissao == EnumPermissaoUsuario.aluno)
                {
                    Aluno a = new Aluno();

                    a.IdUsuario = usuario.IdUsuario;
                    db.Aluno.Add(a);
                }
                else
                {
                    Funcionario f = new Funcionario();

                    f.IdUsuario = usuario.IdUsuario;
                    f.Permissao = usuario.Permissao;

                    db.Funcionario.Add(f);
                }
            }
            catch
            {
                return 0;
            }

            return (db.SaveChanges());
        }

        public bool UsuarioExists(string id)
        {
            return (db.Usuario.Count(e => e.IdUsuario == id) > 0);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}