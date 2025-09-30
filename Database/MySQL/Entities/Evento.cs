using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.MySQL.Entities
{
    [Table("Eventos")]
    public class Evento
    {
        [Key]
        [Column("id_evento")]
        public int IdEvento { get; set; }

        [Column("id_empresa")]
        [Required]
        public int IdEmpresa { get; set; }

        [Column("tipo_evento")]
        [Required]
        public TipoEvento TipoEvento { get; set; }

        [Column("fecha_evento")]
        public DateTime FechaEvento { get; set; } = DateTime.Now;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        // Relaciones
        [ForeignKey("IdEmpresa")]
        public virtual Empresa Empresa { get; set; }
    }
}
