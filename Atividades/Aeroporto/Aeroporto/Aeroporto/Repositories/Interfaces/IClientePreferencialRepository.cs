using SistemaAereo.Models;

namespace SistemaAereo.Repositories.Interfaces
{
    public interface IClientePreferencialRepository : IRepository<ClientePreferencial>
    {
        Task<IEnumerable<ClientePreferencial>> GetClientesAtivosAsync();
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<bool> CPFExistsAsync(string cpf, int? excludeId = null);
        Task<int> GetTotalClientesAtivosAsync();
    }
}