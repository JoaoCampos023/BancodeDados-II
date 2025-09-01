using System;
using Banco_II.Data;
using Banco_II.Models;
using Microsoft.EntityFrameworkCore;

namespace Banco_II.Repository
{
    public class StudentCoursesRepository : IStudentCoursesRepository
    {
        private readonly SchoolContext _context;

        public StudentCoursesRepository(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentCourses>> GetAll()
        {
            return await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .ToListAsync();
        }

        public async Task<StudentCourses> GetById(int studentId, int courseId)
        {
            return await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.StudentID == studentId && sc.CourseID == courseId);
        }

        public async Task Create(StudentCourses studentCourse)
        {
            _context.StudentCourses.Add(studentCourse);
            await _context.SaveChangesAsync();
        }

        public async Task Update(StudentCourses studentCourse)
        {
            _context.StudentCourses.Update(studentCourse);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(StudentCourses studentCourse)
        {
            _context.StudentCourses.Remove(studentCourse);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(int studentId, int courseId)
        {
            return await _context.StudentCourses
                .AnyAsync(sc => sc.StudentID == studentId && sc.CourseID == courseId);
        }
    }
}