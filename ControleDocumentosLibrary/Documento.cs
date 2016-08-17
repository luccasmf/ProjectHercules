namespace ControleDocumentosLibrary
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Documento")]
    public partial class Documento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Documento()
        {
            SolicitacaoDocumento = new HashSet<SolicitacaoDocumento>();
        }

        [Key]
        public int IdDocumento { get; set; }

        public EnumTipoDocumento TipoDocumento { get; set; }

        [Required]
        [StringLength(20)]
        public string NomeDocumento { get; set; }

        public DateTime Data { get; set; }

        [Column("Documento", TypeName = "text")]
        [Required]
        public string CaminhoDocumento { get; set; }

        public int? IdAlunoCurso { get; set; }

        public int? IdEvento { get; set; }

        public virtual AlunoCurso AlunoCurso { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitacaoDocumento> SolicitacaoDocumento { get; set; }

        public virtual Evento Evento { get; set; }
        
        [NotMapped]
        [Required]
        public byte[] arquivo { get; set; }
    }
}
