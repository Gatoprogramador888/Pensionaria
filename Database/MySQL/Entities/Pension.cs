using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.MySQL.Entities
{
    [Table("Pensiones")]
    public class Pension
    {
        [Key]
        [Column("numero_pension")]
        public int NumeroPension { get; set; }

        [Column("ubicacion")]
        [MaxLength(150)]
        public string? Ubicacion { get; set; }

        [Column("capacidad")]
        [MaxLength(50)]
        public string? Capacidad { get; set; }

        [Column("estado")]
        public EstadoGeneral Estado { get; set; } = EstadoGeneral.Disponible;

        // Relaciones
        public virtual ICollection<PensionLog> PensionLogs { get; set; }
    }
}
