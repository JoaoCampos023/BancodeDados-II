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
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
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

        public async Task<bool> AddSubjectAsync(Subject subject)
        {
            try
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log do erro
                Console.WriteLine($"Erro ao adicionar matéria: {ex.Message}");
                return false;
            }
        }
    }
}
