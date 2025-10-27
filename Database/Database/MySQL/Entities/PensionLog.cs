using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Database.MySQL.Entities
{
    [Table("Pensiones_Log")]
    public class PensionLog
    {
        [Key]
        [Column("id_log")]
        public int IdLog { get; set; }

        [Column("numero_pension")]
        [Required]
        public int NumeroPension { get; set; }

        [Column("placa")]
        [Required]
        [MaxLength(20)]
        public string Placa { get; set; }

        [Column("conductor")]
        [Required]
        [MaxLength(100)]
        public string Conductor { get; set; }

        [Column("hora_entrada")]
        [Required]
        public DateTime HoraEntrada { get; set; }

        [Column("hora_salida")]
        public DateTime? HoraSalida { get; set; }

        [Column("estado")]
        public EstadoPensionLog Estado { get; set; } = EstadoPensionLog.Disponible;

        // Relaciones
        [ForeignKey("NumeroPension")]
        public virtual Pension Pension { get; set; }

        [ForeignKey("Placa,Conductor")]
        public virtual Camion Camion { get; set; }
    }
}
