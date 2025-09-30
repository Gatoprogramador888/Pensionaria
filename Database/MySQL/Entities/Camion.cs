using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.MySQL.Entities
{
    [Table("Camion")]
    public class Camion
    {
        [Key]
        [Column("placa")]
        [MaxLength(20)]
        public string Placa { get; set; }

        [Column("tipo")]
        [MaxLength(50)]
        public string? Tipo { get; set; }

        [Column("empresa")]
        public int? Empresa { get; set; }

        [Column("servicio")]
        [Required]
        public ServicioTipo Servicio { get; set; }

        [Column("conductor")]
        [Required]
        [MaxLength(100)]
        public string Conductor { get; set; }

        // Relaciones
        [ForeignKey("Empresa")]
        public virtual Empresa? EmpresaNavigation { get; set; }
        
        public virtual ICollection<AndenLog> AndenLogs { get; set; }
        public virtual ICollection<PensionLog> PensionLogs { get; set; }
    }
}
