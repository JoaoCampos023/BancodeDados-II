using Banco_II.Models;

namespace Banco_II.Repository
{
    public interface IStudentRepository
    {
        public Task Create(Student student);
        public Task Update(Student student);
        public Task Delete(Student student);
        public Task<Student> GetById(int id);
        public Task<List<Student>> GetAll();
        public Task<List<Student>> GetByName(string name);
    }
}
