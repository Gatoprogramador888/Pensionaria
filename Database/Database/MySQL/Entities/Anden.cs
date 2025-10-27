using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Database.MySQL.Entities
{
    [Table("Andenes")]
    public class Anden
    {
        [Key]
        [Column("numero_anden")]
        public int NumeroAnden { get; set; }

        [Column("ubicacion")]
        [MaxLength(150)]
        public string? Ubicacion { get; set; }

        [Column("capacidad")]
        [MaxLength(50)]
        public string? Capacidad { get; set; }

        [Column("estado")]
        public EstadoGeneral Estado { get; set; } = EstadoGeneral.Disponible;

        // Relaciones
        public virtual ICollection<AndenLog> AndenLogs { get; set; }
    }
}
