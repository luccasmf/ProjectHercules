using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ControleDocumentosLibrary;
using ControleDocumentosAPI.Repositories;

namespace ControleDocumentosAPI.Controllers
{
    /// <summary>
    /// Controller para interação com contas de usuário (validação, cadastro, exclusão, alteração)
    /// </summary>
    public class UsuariosController : ApiController
    {

        private UsuarioRepository repo = new UsuarioRepository();

        // GET: api/Usuarios
        /// <summary>
        /// Pega listagem de usuários cadastrados
        /// </summary>
        /// <returns>IQueryable<Usuario></Usuario></returns>
        public IQueryable<Usuario> GetUsuario()
        {
            return repo.GetUsuario();
        }

        // GET: api/Usuarios/5
        /// <summary>
        /// Retorna o usuário solicitado
        /// </summary>
        /// <param name="id">id do usuario desejado</param>
        /// <returns>IHttpActionResult (Ok ou NotFound)</returns>
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult GetUsuario(string id)
        {
            Usuario usuario = repo.GetUsuarioById(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }


        // DELETE: api/Usuarios/5
        /// <summary>
        /// Deleta um usuario
        /// </summary>
        /// <param name="id">id do usuário desejado</param>
        /// <returns>IHttpActionResult (Ok ou NotFound)</returns>
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult DeleteUsuario(string id)
        {
            Usuario usuario = repo.DeleteUsuario(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repo.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioExists(string id)
        {
            return repo.UsuarioExists(id);
        }


        /// <summary>
        /// Verifica se o usuario existe no banco de dados, caso negativo retorna NotFound, caso positivo, retorna o usuário
        /// </summary>
        /// <param name="id">id do usuário (número da carteirinha)</param>
        /// <returns>Retorna NotFound caso o usuário não exista, ou o próprio usuário, caso já esteja cadastrado</returns>
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult GetVerificaUsuario(string id) // GET: api/Usuarios/VerificaBanco?id=11000005405
        {
            if (UsuarioExists(id))
            {
                return Ok(repo.GetUsuarioById(id));
            }
            return NotFound();
        }

        /// <summary>
        /// Método responsável por cadastrar um Usuário na base de dados, separando por permissão
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult PostCadastroUsuario(Usuario usuario)  // POST: api/Usuarios/PostCadastroUsuario
        {
            if (repo.SalvaUsuario(usuario) > 0)
                return Ok(usuario);
            else
                return NotFound();
        }
    }
}