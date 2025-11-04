using SistemaAereo.Models;

namespace SistemaAereo.Repositories.Interfaces
{
    public interface IAeroportoRepository : IRepository<Aeroporto>
    {
        Task<bool> CodigoIATAExistsAsync(string codigoIATA, int? excludeId = null);
        Task<bool> HasVoosAsync(int aeroportoId);
    }
}