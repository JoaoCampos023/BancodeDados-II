using Microsoft.EntityFrameworkCore;
using SistemaAereo.Data;
using SistemaAereo.Models;
using SistemaAereo.Repositories;
using SistemaAereo.Repositories.Interfaces;

public class AeronaveRepository : Repository<Aeronave>, IAeronaveRepository
{
    public AeronaveRepository(AeroportoContext context) : base(context) { }

    public async Task<IEnumerable<Aeronave>> GetAeronavesComVoosAsync()
    {
        return await _dbSet
            .Include(a => a.Voos)
                .ThenInclude(v => v.AeroportoOrigem)
            .Include(a => a.Voos)
                .ThenInclude(v => v.AeroportoDestino)
            .OrderBy(a => a.TipoAeronave)
            .ToListAsync();
    }

    public async Task<bool> HasVoosAsync(int aeronaveId)
    {
        return await _context.Voos.AnyAsync(v => v.AeronaveId == aeronaveId);
    }
}