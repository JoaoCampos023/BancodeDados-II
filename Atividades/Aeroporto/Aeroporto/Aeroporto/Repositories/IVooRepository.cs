using SistemaAereo.Data;
using SistemaAereo.Models;
using Microsoft.EntityFrameworkCore;

namespace SistemaAereo.Repositories
{
    public interface IVooRepository : IRepository<Voo>
    {
        Task<IEnumerable<Voo>> GetVoosCompletosAsync();
        Task<Voo> GetVooCompletoAsync(int id);
        Task<IEnumerable<Voo>> GetProximosVoosAsync(int quantidade = 5);
        Task<bool> NumeroVooExistsAsync(string numeroVoo, int? excludeId = null);
    }

    public class VooRepository : Repository<Voo>, IVooRepository
    {
        public VooRepository(AeroportoContext context) : base(context) { }

        public async Task<IEnumerable<Voo>> GetVoosCompletosAsync()
        {
            return await _dbSet
                .Include(v => v.AeroportoOrigem)
                .Include(v => v.AeroportoDestino)
                .Include(v => v.Aeronave)
                .Include(v => v.Escalas)
                .OrderBy(v => v.HorarioSaida)
                .ToListAsync();
        }

        public async Task<Voo> GetVooCompletoAsync(int id)
        {
            return await _dbSet
                .Include(v => v.AeroportoOrigem)
                .Include(v => v.AeroportoDestino)
                .Include(v => v.Aeronave)
                .Include(v => v.Escalas)
                    .ThenInclude(e => e.Aeroporto)
                .Include(v => v.Poltronas)
                .FirstOrDefaultAsync(v => v.VooId == id);
        }

        public async Task<IEnumerable<Voo>> GetProximosVoosAsync(int quantidade = 5)
        {
            return await _dbSet
                .Include(v => v.AeroportoOrigem)
                .Include(v => v.AeroportoDestino)
                .Where(v => v.HorarioSaida > DateTime.Now)
                .OrderBy(v => v.HorarioSaida)
                .Take(quantidade)
                .ToListAsync();
        }

        public async Task<bool> NumeroVooExistsAsync(string numeroVoo, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return await _dbSet.AnyAsync(v => v.NumeroVoo == numeroVoo && v.VooId != excludeId.Value);

            return await _dbSet.AnyAsync(v => v.NumeroVoo == numeroVoo);
        }
    }
}
