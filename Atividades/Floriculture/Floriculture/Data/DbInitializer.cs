using Floriculture.Models;

namespace Floriculture.Data
{
    public class DbInitializer
    {
        public static void Initialize(FloricultureContext context)
        {
            context.Database.EnsureCreated();

            if (context.Plants.Any())
            {
                return; 
            }

            var plants = new Plant[]
                {
                    new() { PlantName = "Peashooter", SensorValue = 20.5f, SensorEvent = DateTime.Parse("2025-10-14 08:30") },
                    new() { PlantName = "Sunflower", SensorValue = 18.0f, SensorEvent = DateTime.Parse("2025-10-13 09:15") },
                    new() { PlantName = "Cherry Bomb", SensorValue = 22.3f, SensorEvent = DateTime.Parse("2025-10-12 14:45") },
                    new() { PlantName = "Wall-nut", SensorValue = 19.8f, SensorEvent = DateTime.Parse("2025-10-11 16:20") },
                    new() { PlantName = "Cactus", SensorValue = 21.0f, SensorEvent = DateTime.Parse("2025-10-10 11:50") }
                };

            foreach (var plant in plants)
            {
                context.Plants.Add(plant);
            }

            context.SaveChanges();
        }
    }
}
