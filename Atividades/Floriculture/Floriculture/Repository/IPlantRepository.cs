using Floriculture.Models;

namespace Floriculture.Repository
{
    public interface IPlantRepository
    {
        // Operacoes basicas
        public Task Create(Plant plant);
        public Task Update(Plant plant);
        public Task Delete(Plant plant);

        // Consultas
        public Task<Plant?> GetById(int id);
        public Task<List<Plant>> GetAll();
        public Task<List<Plant>> GetByName(string name);
    }
}
