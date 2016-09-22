using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ControleDocumentosLibrary;

namespace ControleDocumentos.Repository
{
    public class TipoDocumentoRepository
    {
        DocumentosModel db = new DocumentosModel();

        /// <summary>
        /// Persisto o novo tipo de documento na base de dados
        /// </summary>
        /// <param name="tipo">nome do novo tipo de documeto</param>
        /// <returns>retorna o tipo cadastrado</returns>
        public TipoDocumento CadastraTipoDoc(string tipo)
        {
            TipoDocumento tipoDoc = new TipoDocumento();
            tipoDoc.TipoDocumento1 = tipo;

            db.TipoDocumento.Add(tipoDoc);

            if (db.SaveChanges() > 1)
            {
                return tipoDoc;
            }
            return null;
        }

        /// <summary>
        /// Deleta o tipo de documento selecionado da base de dados
        /// </summary>
        /// <param name="id">id do tipo selecionado</param>
        /// <returns>retorna verdadeiro ou falso</returns>
        public bool DeletaTipoDoc(int id)
        {
            TipoDocumento tipo = db.TipoDocumento.Find(id);
            db.TipoDocumento.Remove(tipo);

            return db.SaveChanges() > 1;
        }

        /// <summary>
        /// Busca todos os tipos de documento já cadastrados no sistema
        /// </summary>
        /// <returns>retorna um array de tipos de documento</returns>
        public TipoDocumento[] listaTipos()
        {
            return db.TipoDocumento.ToArray();
        }


        /// <summary>
        /// Retorna o tipo do id buscado
        /// </summary>
        /// <param name="id"></param>
        /// <returns>retorna o tipo de documento desejado</returns>
        public TipoDocumento GetTipoDoc(int? id)
        {
            if (id.HasValue)
                return db.TipoDocumento.Find(id);
            return null;
        }

    }
}