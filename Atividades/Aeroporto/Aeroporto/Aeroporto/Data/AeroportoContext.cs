using SistemaAereo.Models;
using Microsoft.EntityFrameworkCore;

namespace SistemaAereo.Data
{
    public class AeroportoContext : DbContext
    {
        public AeroportoContext(DbContextOptions<AeroportoContext> options) : base(options)
        {
        }

        public DbSet<Aeronave> Aeronaves { get; set; }
        public DbSet<Models.Aeroporto> Aeroportos { get; set; }
        public DbSet<Voo> Voos { get; set; }
        public DbSet<Escala> Escalas { get; set; }
        public DbSet<Poltrona> Poltronas { get; set; }
        public DbSet<ClientePreferencial> ClientesPreferenciais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações de relacionamento
            modelBuilder.Entity<Voo>()
                .HasOne(v => v.AeroportoOrigem)
                .WithMany(a => a.VoosOrigem)
                .HasForeignKey(v => v.AeroportoOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voo>()
                .HasOne(v => v.AeroportoDestino)
                .WithMany(a => a.VoosDestino)
                .HasForeignKey(v => v.AeroportoDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voo>()
                .HasOne(v => v.Aeronave)
                .WithMany(a => a.Voos)
                .HasForeignKey(v => v.AeronaveId);

            modelBuilder.Entity<Escala>()
                .HasOne(e => e.Voo)
                .WithMany(v => v.Escalas)
                .HasForeignKey(e => e.VooId);

            modelBuilder.Entity<Escala>()
                .HasOne(e => e.Aeroporto)
                .WithMany(a => a.Escalas)
                .HasForeignKey(e => e.AeroportoId);

            modelBuilder.Entity<Poltrona>()
                .HasOne(p => p.Voo)
                .WithMany(v => v.Poltronas)
                .HasForeignKey(p => p.VooId);

            // Configurações adicionais
            modelBuilder.Entity<ClientePreferencial>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<ClientePreferencial>()
                .HasIndex(c => c.CPF)
                .IsUnique();

            modelBuilder.Entity<Models.Aeroporto>()
                .HasIndex(a => a.CodigoIATA)
                .IsUnique();

            modelBuilder.Entity<Voo>()
                .HasIndex(v => v.NumeroVoo)
                .IsUnique();
        }
    }
}
