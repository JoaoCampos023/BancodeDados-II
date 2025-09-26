using Banco_II.Models;

namespace Banco_II.Repository
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAll();
        Task<Subject> GetById(int id);
        Task Create(Subject subject);
        Task Update(Subject subject);
        Task Delete(Subject subject);
        Task<IEnumerable<Subject>> GetSubjectsByCourseId(int courseId);
        Task<IEnumerable<Subject>> GetAllWithCourses();
    }
}