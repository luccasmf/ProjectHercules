using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;
using System.Reflection;
using ControleDocumentos.Util;

namespace ControleDocumentos.Repository
{
    public class UsuarioRepository
    {
        DocumentosModel db = new DocumentosModel();

        public List<Usuario> GetUsuarios()
        {
            return db.Usuario.ToList();
        }

        public Usuario GetUsuarioByALuno(Aluno al)
        {
            return db.Usuario.Find(al.IdUsuario);
        }

        public Usuario GetUsuarioByFuncionario(Funcionario func)
        {
            return db.Usuario.Find(func.IdUsuario);
        }

        public Usuario GetUsuarioById(string idUsuario)
        {
            return db.Usuario.Find(idUsuario);
        }

        public string PersisteUsuario(Usuario user)
        {
            if (!string.IsNullOrEmpty(user.IdUsuario))
            {
                return ComparaInfos(user);
            }
            else
            {
                db.Usuario.Add(user);

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

        private string ComparaInfos(Usuario user)
        {
            Usuario userOld = db.Usuario.Find(user.IdUsuario);

            userOld = Utilidades.ComparaValores(userOld, user, new string[] {"Nome","E_mail","Permissao"});

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