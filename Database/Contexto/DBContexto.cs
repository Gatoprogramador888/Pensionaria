using Microsoft.EntityFrameworkCore;

namespace Contexto
{
    public class DBContexto : DbContext
    {
        public DBContexto(DbContextOptions<DBContexto> options) : base(options) { }

        // DbSets for entities
        public DbSet<Entities.Empresa> Empresas { get; set; }
        public DbSet<Entities.Camion> Camiones { get; set; }
        public DbSet<Entities.Anden> Andenes { get; set; }
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
    }
}
