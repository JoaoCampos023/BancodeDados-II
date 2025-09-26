using System;
using Banco_II.Data;
using Banco_II.Models;
using Microsoft.EntityFrameworkCore;

namespace Banco_II.Repository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly SchoolContext _context;

        public SubjectRepository(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAll()
        {
            return await _context.Subjects.Include(s => s.Course).ToListAsync();
        }

        public async Task<Subject> GetById(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task Create(Subject subject)
        {
            try
            {
                Console.WriteLine($"=== TENTATIVA DE CRIAR MATÉRIA ===");
                Console.WriteLine($"Nome: {subject.Name}");
                Console.WriteLine($"CourseID: {subject.CourseID}");

                // VERIFICAR SE O CURSO EXISTE
                var course = await _context.Courses.FindAsync(subject.CourseID);
                if (course == null)
                {
                    throw new ArgumentException($"Curso com ID {subject.CourseID} não existe!");
                }

                Console.WriteLine($"Curso encontrado: {course.Name}");

                // VERIFICAR DUPLICATA
                var existingSubject = await _context.Subjects
                    .FirstOrDefaultAsync(s => s.Name == subject.Name && s.CourseID == subject.CourseID);

                if (existingSubject != null)
                {
                    throw new InvalidOperationException($"Já existe uma matéria com o nome '{subject.Name}' no curso '{course.Name}'");
                }

                // CORREÇÃO: Garantir que a matéria está associada ao curso
                subject.Course = course; // Isso força o relacionamento

                // SALVAR
                await _context.Subjects.AddAsync(subject);
                await _context.SaveChangesAsync();

                Console.WriteLine($"=== MATÉRIA SALVA COM SUCESSO ===");
                Console.WriteLine($"ID da matéria: {subject.ID}");
                Console.WriteLine($"Curso associado: {subject.Course?.Name}");

                // VERIFICAÇÃO FINAL
                var savedSubject = await _context.Subjects
                    .Include(s => s.Course)
                    .FirstOrDefaultAsync(s => s.ID == subject.ID);

                Console.WriteLine($"Verificação pós-salvamento: {(savedSubject != null ? "SUCESSO" : "FALHA")}");
                Console.WriteLine($"Curso na verificação: {savedSubject?.Course?.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERRO AO CRIAR MATÉRIA ===");
                Console.WriteLine($"Mensagem: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
        public async Task Update(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Subject subject)
        {
            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByCourseId(int courseId)
        {
            return await _context.Subjects
                .Where(s => s.CourseID == courseId)
                .Include(s => s.Course)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetAllWithCourses()
        {
            return await _context.Subjects
                .Include(s => s.Course)
                .OrderBy(s => s.Course.Name)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }
    }
}
