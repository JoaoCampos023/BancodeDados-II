using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Scaffolding.Models;

namespace Scaffolding.Data;

public partial class UnoescBd2Context : DbContext
{
    public UnoescBd2Context()
    {
    }

    public UnoescBd2Context(DbContextOptions<UnoescBd2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseModule> CourseModules { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentCourse> StudentCourses { get; set; }

    public virtual DbSet<StudentModule> StudentModules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UnoescBD2");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<CourseModule>(entity =>
        {
            entity.HasKey(e => new { e.CourseId, e.ModuleId });

            entity.ToTable("CourseModule");

            entity.HasIndex(e => e.ModuleId, "IX_CourseModule_ModuleID");

            entity.Property(e => e.CourseId).HasColumnName("CourseID");
            entity.Property(e => e.ModuleId).HasColumnName("ModuleID");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseModules).HasForeignKey(d => d.CourseId);

            entity.HasOne(d => d.Module).WithMany(p => p.CourseModules).HasForeignKey(d => d.ModuleId);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.ToTable("Module");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.HasMany(d => d.Modules).WithMany(p => p.Prerequisites)
                .UsingEntity<Dictionary<string, object>>(
                    "ModulePrerequisite",
                    r => r.HasOne<Module>().WithMany()
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Module>().WithMany()
                        .HasForeignKey("PrerequisiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ModuleId", "PrerequisiteId");
                        j.ToTable("ModulePrerequisite");
                        j.HasIndex(new[] { "PrerequisiteId" }, "IX_ModulePrerequisite_PrerequisiteID");
                        j.IndexerProperty<int>("ModuleId").HasColumnName("ModuleID");
                        j.IndexerProperty<int>("PrerequisiteId").HasColumnName("PrerequisiteID");
                    });

            entity.HasMany(d => d.Prerequisites).WithMany(p => p.Modules)
                .UsingEntity<Dictionary<string, object>>(
                    "ModulePrerequisite",
                    r => r.HasOne<Module>().WithMany()
                        .HasForeignKey("PrerequisiteId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Module>().WithMany()
                        .HasForeignKey("ModuleId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("ModuleId", "PrerequisiteId");
                        j.ToTable("ModulePrerequisite");
                        j.HasIndex(new[] { "PrerequisiteId" }, "IX_ModulePrerequisite_PrerequisiteID");
                        j.IndexerProperty<int>("ModuleId").HasColumnName("ModuleID");
                        j.IndexerProperty<int>("PrerequisiteId").HasColumnName("PrerequisiteID");
                    });
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Student");

            entity.Property(e => e.Id).HasColumnName("ID");
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.CourseId });

            entity.ToTable("StudentCourse");

            entity.HasIndex(e => e.CourseId, "IX_StudentCourse_CourseID");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");

            entity.HasOne(d => d.Course).WithMany(p => p.StudentCourses).HasForeignKey(d => d.CourseId);

            entity.HasOne(d => d.Student).WithMany(p => p.StudentCourses).HasForeignKey(d => d.StudentId);
        });

        modelBuilder.Entity<StudentModule>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ModuleId, e.CourseId });

            entity.ToTable("StudentModule");

            entity.HasIndex(e => e.CourseId, "IX_StudentModule_CourseID");

            entity.HasIndex(e => e.ModuleId, "IX_StudentModule_ModuleID");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
            entity.Property(e => e.CourseId).HasColumnName("CourseID");

            entity.HasOne(d => d.Course).WithMany(p => p.StudentModules).HasForeignKey(d => d.CourseId);

            entity.HasOne(d => d.Module).WithMany(p => p.StudentModules).HasForeignKey(d => d.ModuleId);

            entity.HasOne(d => d.Student).WithMany(p => p.StudentModules).HasForeignKey(d => d.StudentId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
