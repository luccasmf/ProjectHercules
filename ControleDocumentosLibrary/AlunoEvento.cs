namespace ControleDocumentosLibrary
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AlunoEvento")]
    public partial class AlunoEvento
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdAluno { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEvento { get; set; }

        public int Presenca { get; set; }

        public virtual Aluno Aluno { get; set; }

        public virtual Evento Evento { get; set; }
    }
}
