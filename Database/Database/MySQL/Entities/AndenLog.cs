using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Database.MySQL.Entities
{
    [Table("Andenes_Log")]
    public class AndenLog
    {
        [Key]
        [Column("id_log")]
        public int IdLog { get; set; }

        [Column("numero_anden")]
        [Required]
        public int NumeroAnden { get; set; }

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
        public EstadoLog Estado { get; set; } = EstadoLog.Proceso;

        // Relaciones
        [ForeignKey("NumeroAnden")]
        public virtual Anden Anden { get; set; }

        [ForeignKey("Placa,Conductor")]
        public virtual Camion Camion { get; set; }
    }
}
