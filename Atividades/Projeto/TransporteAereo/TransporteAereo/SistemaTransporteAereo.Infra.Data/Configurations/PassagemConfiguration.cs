using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using SistemaTransporteAereo.Domain.Entities;

namespace SistemaTransporteAereo.Infra.Data.Configurations
{
    public class PassagemConfiguration : EntityTypeConfiguration<Passagem>
    {
        public PassagemConfiguration()
        {
            HasKey(p => p.PassagemId);

            Property(p => p.CodigoReserva)
                .IsRequired()
                .HasMaxLength(20);

            Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(20);

            Property(p => p.NumeroPoltrona)
                .HasMaxLength(10);

            Property(p => p.LocalEmbarque)
                .HasMaxLength(50);

            Property(p => p.PesoMala)
                .HasPrecision(5, 2);

            HasIndex(p => p.CodigoReserva)
                .IsUnique();

            HasRequired(p => p.Cliente)
                .WithMany(c => c.Passagens)
                .HasForeignKey(p => p.ClienteId);

            HasRequired(p => p.Voo)
                .WithMany(v => v.Passagens)
                .HasForeignKey(p => p.VooId);
        }
    }
}