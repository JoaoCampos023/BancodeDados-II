using Microsoft.EntityFrameworkCore;
using Imobiliaria.Models;

namespace Imobiliaria.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Imovel> Imoveis { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Imovel>().ToTable("Imovel");
        }
    }
}
