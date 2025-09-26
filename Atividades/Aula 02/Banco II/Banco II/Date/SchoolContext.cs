using Banco_II.Models;
using Microsoft.EntityFrameworkCore;

namespace Banco_II.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
        {}

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourses> StudentCourses { get; set; }
        public DbSet<Subject> Subjects { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar a chave composta para StudentCourses
            modelBuilder.Entity<StudentCourses>()
                .HasKey(sc => new { sc.StudentID, sc.CourseID });

            // CORREÇÃO: Configurar relação Course -> Subjects CORRETAMENTE
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Subjects)
                .WithOne(s => s.Course)
                .HasForeignKey(s => s.CourseID)
                .OnDelete(DeleteBehavior.Cascade); // Isso garante que se o curso for deletado, as matérias também serão

            base.OnModelCreating(modelBuilder);
        }

    }
}
