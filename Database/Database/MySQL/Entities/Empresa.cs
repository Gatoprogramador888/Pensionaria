using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Database.MySQL.Entities
{
    [Table("Empresa")]
    public class Empresa
    {
        [Key]
        [Column("id_empresa")]
        public int IdEmpresa { get; set; }

        [Column("nombre_empresa")]
        [Required]
        [MaxLength(150)]
        public string NombreEmpresa { get; set; }

        [Column("numero_telefonico")]
        [MaxLength(20)]
        public string? NumeroTelefonico { get; set; }

        [Column("correo")]
        [MaxLength(150)]
        public string? Correo { get; set; }

        [Column("fecha_afiliacion")]
        public DateTime? FechaAfiliacion { get; set; }

        [Column("fecha_expiracion_afiliacion")]
        public DateTime? FechaExpiracionAfiliacion { get; set; }

        [Column("descuento", TypeName = "json")]
        public string? Descuento { get; set; }

        // Relaciones
        public virtual ICollection<Camion> Camiones { get; set; }
        public virtual ICollection<Evento> Eventos { get; set; }
    }

}
