using SistemaAereo.Models;

namespace SistemaAereo.Repositories.Interfaces
{
    public interface IClientePreferencialRepository : IRepository<ClientePreferencial>
    {
        // =============================================
        // CONSULTAS DE CLIENTES
        // =============================================

        Task<IEnumerable<ClientePreferencial>> GetClientesAtivosAsync();
        Task<int> GetTotalClientesAtivosAsync();

        // =============================================
        // VALIDAÇÕES DE UNICIDADE
        // =============================================

        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<bool> CPFExistsAsync(string cpf, int? excludeId = null);
    }
}