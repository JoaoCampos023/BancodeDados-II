using Floriculture.Models;
using Microsoft.EntityFrameworkCore;

namespace Floriculture.Data
{
    public class FloricultureContext : DbContext
    {
        public FloricultureContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Plant> Plants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plant>().ToTable("Plant");
        }
    }
}
