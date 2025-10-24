using Floriculture.Data;
using Floriculture.Models;
using Microsoft.EntityFrameworkCore;

namespace Floriculture.Repository
{
    public class PlantRepository : IPlantRepository
    {
        private readonly FloricultureContext _context;
        public PlantRepository(FloricultureContext context) => _context = context;

        #region Basic Operations
        public async Task Create(Plant plant)
        {
            await _context.Plants.AddAsync(plant);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Plant plant)
        {
            var existingPlant = await _context.Plants
                .FirstOrDefaultAsync(p => p.Id == plant.Id);

            if (existingPlant != null)
            {
                _context.Entry(existingPlant).CurrentValues.SetValues(plant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(Plant plant)
        {
            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Queries
        public async Task<Plant?> GetById(int id)
        {
            var plant = await _context.Plants
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            return plant;
        }

        public async Task<List<Plant>> GetAll()
        {
            var data = await _context.Plants.ToListAsync();
            return data;
        }

        public async Task<List<Plant>> GetByName(string name)
        {
            var plants = await _context.Plants
                .Where(p => p.PlantName != null &&
                    // Nao diferencia o case
                    p.PlantName.Contains(name, StringComparison.CurrentCultureIgnoreCase))
                .ToListAsync();

            return plants;
        }
        #endregion
    }
}
