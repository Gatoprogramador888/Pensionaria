using Database.MySQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.MySQL.Contexto
{
    public class DBContexto : DbContext
    {
        public DBContexto(DbContextOptions<DBContexto> options) : base(options) { }

        // DbSets for entities
        public DbSet<Entities.Empresa> Empresas { get; set; }
        public DbSet<Entities.Camion> Camiones { get; set; }
        public DbSet<Anden> Andenes { get; set; }
        public DbSet<Entities.AndenLog> AndenLogs { get; set; }
        public DbSet<Entities.Pension> Pensiones { get; set; }
        public DbSet<Entities.PensionLog> PensionLogs { get; set; }
        public DbSet<Entities.Evento> Eventos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(
                    "server=localhost;database=pensionaria;user=root;password=",
                    new MySqlServerVersion(new Version(8, 0, 0))
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Camion con clave compuesta (placa, conductor)
            modelBuilder.Entity<Entities.Camion>()
                .HasKey(c => new { c.Placa, c.Conductor });

            // Configuración de AndenLog con clave compuesta foránea
            modelBuilder.Entity<Entities.AndenLog>()
                .HasOne(al => al.Camion)
                .WithMany(c => c.AndenLogs)
                .HasForeignKey(al => new { al.Placa, al.Conductor })
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de PensionLog con clave compuesta foránea
            modelBuilder.Entity<Entities.PensionLog>()
                .HasOne(pl => pl.Camion)
                .WithMany(c => c.PensionLogs)
                .HasForeignKey(pl => new { pl.Placa, pl.Conductor })
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de conversiones de ENUM a string para MySQL
            modelBuilder.Entity<Entities.Camion>()
                .Property(c => c.Servicio)
                .HasConversion(
                    v => v.ToString().ToLower().Replace("cd", "c/d"),
                    v => v == "c/d" ? Entities.ServicioTipo.CD :
                        Enum.Parse<Entities.ServicioTipo>(v, true)
                );

            modelBuilder.Entity<Anden>()
                .Property(a => a.Estado)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<Entities.EstadoGeneral>(v, true)
                );

            modelBuilder.Entity<Entities.AndenLog>()
                .Property(al => al.Estado)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<Entities.EstadoLog>(v, true)
                );

            modelBuilder.Entity<Entities.Pension>()
                .Property(p => p.Estado)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<Entities.EstadoGeneral>(v, true)
                );

            modelBuilder.Entity<Entities.PensionLog>()
                .Property(pl => pl.Estado)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<Entities.EstadoPensionLog>(v, true)
                );

            modelBuilder.Entity<Entities.Evento>()
                .Property(e => e.TipoEvento)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<Entities.TipoEvento>(v, true)
                );
        }

    }
}
