using SistemaAereo.Data;
using SistemaAereo.Models;
using Microsoft.EntityFrameworkCore;

namespace SistemaAereo.Repositories
{
    public interface IAeronaveRepository : IRepository<Aeronave>
    {
        Task<IEnumerable<Aeronave>> GetAeronavesComVoosAsync();
        Task<bool> HasVoosAsync(int aeronaveId);
    }

    public class AeronaveRepository : Repository<Aeronave>, IAeronaveRepository
    {
        public AeronaveRepository(AeroportoContext context) : base(context) { }

        public async Task<IEnumerable<Aeronave>> GetAeronavesComVoosAsync()
        {
            return await _dbSet.Include(a => a.Voos).OrderBy(a => a.TipoAeronave).ToListAsync();
        }

        public async Task<bool> HasVoosAsync(int aeronaveId)
        {
            return await _context.Voos.AnyAsync(v => v.AeronaveId == aeronaveId);
        }
    }
}
