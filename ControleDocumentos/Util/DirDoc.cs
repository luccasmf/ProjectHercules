using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using ControleDocumentosLibrary;
using System.Security.Cryptography;
using System.Text;
using ControleDocumentos.Repository;

namespace ControleDocumentos
{
    /// <summary>
    /// Classe responsável por manipular os documentos, separar em pastas e salvar no servidor de arquivos
    /// </summary>
    public static class DirDoc
    {
        private static DocumentoRepository documentoRepository = new DocumentoRepository();

        // key for encryption
        static byte[] Key = Encoding.UTF8.GetBytes("qA!$p(SKJkOK}s&~lZZ4E87s{_6Y9Wv7YZc.q/C1{10_l9!Hk&yI&I<.#4");
        private static string caminhoPadrao = @".//Documentos/";
        private static string caminhoDownload = caminhoPadrao + "Download/";

        /// <summary>
        /// Salva o arquivo enviado
        /// </summary>
        /// <param name="doc">documento a ser salvo</param>
        /// <returns>Documento com endereço e nome corretos para inserção no banco de dados</returns>
        public static string SalvaArquivo(Documento doc)
        {

            string curso = doc.AlunoCurso.Curso.Nome;
            string idAluno = doc.AlunoCurso.IdAluno.ToString();
            string tipoDoc = doc.TipoDocumento.TipoDocumento1;


            List<string> caminho = new List<string>();

            caminho.Add(curso);
            caminho.Add(idAluno);
            caminho.Add(tipoDoc);

            CriaDiretorio(caminho.ToArray());
            doc.NomeDocumento = GeraNomeArquivo(doc.NomeDocumento);

            string outputFile = CriaDiretorio(caminho.ToArray()) + doc.NomeDocumento;
            doc.CaminhoDocumento = outputFile;

           try
            {
                if (File.Exists(outputFile))
                {
                    return "Arquivo existente";
                }
                else
                {
                    FileStream fs = new FileStream(outputFile, FileMode.Create);
                    RijndaelManaged rmCryp = new RijndaelManaged();
                    CryptoStream cs = new CryptoStream(fs, rmCryp.CreateEncryptor(Key, Key), CryptoStreamMode.Write);

                    foreach (var data in doc.arquivo)
                    {
                        cs.WriteByte((byte)data);
                    }
                    cs.Close();
                    fs.Close();
                }

                if (documentoRepository.PersisteDocumento(doc))
                {
                    return "Sucesso";
                }

                return "Falha ao persistir";
                
            }
            catch
            {
                return "Falha ao criptografar";
            }
            
            
        }
        /// <summary>
        /// Desencripta arquivo para download
        /// </summary>
        /// <param name="nomeArquivo">nome do arquivo desejado</param>
        /// <returns>um caminho temporário para baixar o arquivo</returns>
        public static byte[] BaixaArquivo(string nomeArquivo)
        {
            Documento doc = documentoRepository.GetDocumentoByNome(nomeArquivo);
            FileStream fs = new FileStream(doc.CaminhoDocumento, FileMode.Open);
            RijndaelManaged rmCryp = new RijndaelManaged();
            CryptoStream cs = new CryptoStream(fs, rmCryp.CreateDecryptor(Key, Key), CryptoStreamMode.Read);
            string caminho = caminhoDownload + doc.AlunoCurso.IdAluno + doc.NomeDocumento;
            StreamWriter fsDecrypted = new StreamWriter(caminho);
            fsDecrypted.Write(new StreamReader(cs).ReadToEnd());
            fsDecrypted.Flush();
            fsDecrypted.Close();
            fs.Close();
            cs.Close();

            byte[] arquivo = File.ReadAllBytes(caminho);
            File.Delete(caminho);

            return arquivo;

        }

        /// <summary>
        /// Deleta o arquivo especificado
        /// </summary>
        /// <param name="caminhoArquivo">caminho do arquivo</param>
        /// <returns>retorna true ou false</returns>
        public static bool DeletaArquivo(string caminhoArquivo)
        {
            try
            {
                File.Delete(caminhoArquivo);
                return true;
            }
            catch
            {
                return false;
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
            string formato = Path.GetExtension(nomeAntigo);

            novoNome = string.Format("{0}{1}{2}{3}", dt.Year, dt.Month, dt.Day, formato);

            return novoNome;

        }
    }
}