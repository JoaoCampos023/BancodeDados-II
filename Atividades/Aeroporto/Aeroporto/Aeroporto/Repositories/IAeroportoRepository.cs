using SistemaAereo.Models;
using SistemaAereo.Data;
using Microsoft.EntityFrameworkCore;

namespace SistemaAereo.Repositories
{
    public interface IAeroportoRepository : IRepository<Aeroporto>
    {
        Task<bool> CodigoIATAExistsAsync(string codigoIATA, int? excludeId = null);
        Task<bool> HasVoosAsync(int aeroportoId);
    }

    public class AeroportoRepository : Repository<Aeroporto>, IAeroportoRepository
    {
        public AeroportoRepository(AeroportoContext context) : base(context) { }

        public async Task<bool> CodigoIATAExistsAsync(string codigoIATA, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return await _dbSet.AnyAsync(a => a.CodigoIATA == codigoIATA && a.AeroportoId != excludeId.Value);

            return await _dbSet.AnyAsync(a => a.CodigoIATA == codigoIATA);
        }

        public async Task<bool> HasVoosAsync(int aeroportoId)
        {
            return await _context.Voos.AnyAsync(v => v.AeroportoOrigemId == aeroportoId || v.AeroportoDestinoId == aeroportoId);
        }
    }
}
