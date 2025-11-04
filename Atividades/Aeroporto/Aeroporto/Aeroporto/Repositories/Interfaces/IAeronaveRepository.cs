using SistemaAereo.Models;

namespace SistemaAereo.Repositories.Interfaces
{
    public interface IAeronaveRepository : IRepository<Aeronave>
    {
        Task<IEnumerable<Aeronave>> GetAeronavesComVoosAsync();
        Task<bool> HasVoosAsync(int aeronaveId);
    }
}