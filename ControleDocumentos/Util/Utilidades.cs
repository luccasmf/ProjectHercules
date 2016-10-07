using ControleDocumentos.Models;
using ControleDocumentos.Repository;
using ControleDocumentosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControleDocumentos.Util
{
    public static class Utilidades
    {
       private static UsuarioRepository usuarioRepository = new UsuarioRepository();

        /// <summary>
        /// Compara propriedades dos objetos passados e caso haja mudanças, substitui os valores e retorna o "atualizado"
        /// </summary>
        /// <typeparam name="T">tipo do objeto</typeparam>
        /// <param name="old">versão persistida do objeto</param>
        /// <param name="to">nova versão do objeto</param>
        /// <param name="alterar">nome dos campos a serem alterados (string[])</param>
        /// <returns>retorna o objeto com as informações atualizadas</returns>
        public static T ComparaValores<T>(T old, T to, params string[] alterar) where T : class
        {
            if (old != null && to != null)
            {
                Type type = typeof(T);
                List<string> alterarList = new List<string>(alterar);
                foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (alterar.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(old, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                       if(!(selfValue == toValue))
                        {
                            pi.SetValue(old, toValue);
                        }
                    }
                }
            }
            return old;
        }

        public static Usuario GetSession(LoginModel ob)
        {
            Usuario us = usuarioRepository.GetUsuarioById(ob.UserName);

            return us;
        }
    }
}