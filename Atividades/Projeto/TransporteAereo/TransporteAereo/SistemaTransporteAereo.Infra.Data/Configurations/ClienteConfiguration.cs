using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using SistemaTransporteAereo.Domain.Entities;

namespace SistemaTransporteAereo.Infra.Data.Configurations
{
    public class ClienteConfiguration : EntityTypeConfiguration<Cliente>
    {
        public ClienteConfiguration()
        {
            HasKey(c => c.ClienteId);

            Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            Property(c => c.CPF)
                .IsRequired()
                .HasMaxLength(11);

            Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            Property(c => c.Telefone)
                .HasMaxLength(20);

            Property(c => c.DataCadastro)
                .IsRequired();

            HasIndex(c => c.CPF)
                .IsUnique();

            HasIndex(c => c.Email)
                .IsUnique();

            HasMany(c => c.Passagens)
                .WithRequired(p => p.Cliente)
                .HasForeignKey(p => p.ClienteId);
        }
    }
}