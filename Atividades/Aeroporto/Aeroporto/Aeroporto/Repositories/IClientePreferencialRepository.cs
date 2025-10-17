using SistemaAereo.Data;
using SistemaAereo.Models;
using Microsoft.EntityFrameworkCore;

namespace SistemaAereo.Repositories
{
    public interface IClientePreferencialRepository : IRepository<ClientePreferencial>
    {
        Task<IEnumerable<ClientePreferencial>> GetClientesAtivosAsync();
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<bool> CPFExistsAsync(string cpf, int? excludeId = null);
        Task<int> GetTotalClientesAtivosAsync();
    }

    public class ClientePreferencialRepository : Repository<ClientePreferencial>, IClientePreferencialRepository
    {
        public ClientePreferencialRepository(AeroportoContext context) : base(context) { }

        public async Task<IEnumerable<ClientePreferencial>> GetClientesAtivosAsync()
        {
            return await _dbSet.Where(c => c.Ativo).OrderBy(c => c.Nome).ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return await _dbSet.AnyAsync(c => c.Email == email && c.ClienteId != excludeId.Value);

            return await _dbSet.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> CPFExistsAsync(string cpf, int? excludeId = null)
        {
            if (string.IsNullOrEmpty(cpf)) return false;

            if (excludeId.HasValue)
                return await _dbSet.AnyAsync(c => c.CPF == cpf && c.ClienteId != excludeId.Value);

            return await _dbSet.AnyAsync(c => c.CPF == cpf);
        }

        public async Task<int> GetTotalClientesAtivosAsync()
        {
            return await _dbSet.CountAsync(c => c.Ativo);
        }
    }
}
