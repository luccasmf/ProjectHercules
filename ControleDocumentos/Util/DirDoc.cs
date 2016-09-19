using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ControleDocumentosLibrary;

namespace ControleDocumentos
{
    /// <summary>
    /// Classe responsável por manipular os documentos, separar em pastas e salvar no servidor de arquivos
    /// </summary>
    public static class DirDoc
    {
        private static string caminhoPadrao = @".//Documentos/";

        /// <summary>
        /// Salva o arquivo enviado
        /// </summary>
        /// <param name="doc">documento a ser salvo</param>
        /// <returns>Documento com endereço e nome corretos para inserção no banco de dados</returns>
        public static Documento SalvaArquivo(Documento doc)
        {

            string curso = doc.AlunoCurso.Curso.Nome;
            string idAluno = doc.AlunoCurso.IdAluno.ToString();
            string tipoDoc = ((EnumTipoDocumento)doc.TipoDocumento).ToString();

            List<string> caminho = new List<string>();

            caminho.Add(curso);
            caminho.Add(idAluno);
            caminho.Add(tipoDoc);

            string path = CriaDiretorio(caminho.ToArray());
            doc.NomeDocumento = GeraNomeArquivo(doc.NomeDocumento);


            string caminhoSalvar = Path.Combine(path, doc.NomeDocumento);
            try
            {
                File.WriteAllBytes(caminhoSalvar, doc.arquivo);
                doc.CaminhoDocumento = caminhoSalvar;
                return doc;
            }
            catch
            {
                return null;
                    
            }
        }

        /// <summary>
        /// Verifica a existencia do diretorio, caso não exista, cria
        /// </summary>
        /// <param name="dir">array com a ordem dos diretorios a serem criados</param>
        /// <returns>string com o endereço final</returns>
        private static string CriaDiretorio(string[] dir)
        {
            string pasta = caminhoPadrao;

            for (int i = 0; i < dir.Length-1; i++)
            {
                pasta += dir[i] + "/";
                if (!Directory.Exists(pasta))
                {
                    //Criamos um com o nome folder
                    Directory.CreateDirectory(pasta);

                }
            }

            return pasta;
        }

        /// <summary>
        /// Pega as informações MIME do arquivo
        /// </summary>
        /// <param name="nomeArquivo">nome do Arquivo como vem do formulario de inserção</param>
        /// <returns>string de informações MIME</returns>
        private static string GetMime(string nomeArquivo)
        {
            return System.Web.MimeMapping.GetMimeMapping(nomeArquivo);
        }

        /// <summary>
        /// Gera novo nome para o arquivo, baseado na data, mantendo formato padrão
        /// </summary>
        /// <param name="nomeAntigo">nome inicial do arquivo</param>
        /// <returns>string com novo nome</returns>
        private static string GeraNomeArquivo(string nomeAntigo)
        {
            string novoNome = "";
            DateTime dt = DateTime.Now;
            string formato = novoNome.Substring(novoNome.Length-4);

            novoNome = string.Format("{0}{1}{2}{3}", dt.Year, dt.Month, dt.Day, formato);

            return novoNome;

        }
    }
}