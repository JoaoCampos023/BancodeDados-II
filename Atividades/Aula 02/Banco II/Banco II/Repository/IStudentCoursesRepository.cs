using Banco_II.Models;

namespace Banco_II.Repository
{
    public interface IStudentCoursesRepository
    {
        Task<IEnumerable<StudentCourses>> GetAll();
        Task<StudentCourses> GetById(int studentId, int courseId);
        Task Create(StudentCourses studentCourse);
        Task Update(StudentCourses studentCourse);
        Task Delete(StudentCourses studentCourse);
        Task<bool> Exists(int studentId, int courseId);
    }
}