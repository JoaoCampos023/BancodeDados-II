using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using SistemaTransporteAereo.Domain.Entities;

namespace SistemaTransporteAereo.Infra.Data.Configurations
{
    public class AeroportoConfiguration : EntityTypeConfiguration<Aeroporto>
    {
        public AeroportoConfiguration()
        {
            HasKey(a => a.AeroportoId);

            Property(a => a.Codigo)
                .IsRequired()
                .HasMaxLength(10);

            Property(a => a.Nome)
                .IsRequired()
                .HasMaxLength(100);

            Property(a => a.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            Property(a => a.Pais)
                .IsRequired()
                .HasMaxLength(50);

            HasIndex(a => a.Codigo)
                .IsUnique();

            HasMany(a => a.VoosOrigem)
                .WithRequired(v => v.AeroportoOrigem)
                .HasForeignKey(v => v.AeroportoOrigemId)
                .WillCascadeOnDelete(false);

            HasMany(a => a.VoosDestino)
                .WithRequired(v => v.AeroportoDestino)
                .HasForeignKey(v => v.AeroportoDestinoId)
                .WillCascadeOnDelete(false);
        }
    }
}