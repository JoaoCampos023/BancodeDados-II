using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.EntityFrameworkCore;
using SistemaTransporteAereo.Domain.Entities;
using SistemaTransporteAereo.Infra.Data.Configurations;

namespace SistemaTransporteAereo.Infra.Data.Context
{
    public class TransporteAereoContext : DbContext
    {
        public TransporteAereoContext() : base("TransporteAereoConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Aeroporto> Aeroportos { get; set; }
        public DbSet<Aviao> Avioes { get; set; }
        public DbSet<Voo> Voos { get; set; }
        public DbSet<Passagem> Passagens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Properties<string>()
                .Configure(p => p.HasColumnType("varchar"));

            modelBuilder.Properties<string>()
                .Configure(p => p.HasMaxLength(100));

            modelBuilder.Configurations.Add(new ClienteConfiguration());
            modelBuilder.Configurations.Add(new AeroportoConfiguration());
            modelBuilder.Configurations.Add(new AviaoConfiguration());
            modelBuilder.Configurations.Add(new VooConfiguration());
            modelBuilder.Configurations.Add(new PassagemConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}