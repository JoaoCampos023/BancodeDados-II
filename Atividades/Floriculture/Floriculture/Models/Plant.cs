using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Floriculture.Models
{
    public class Plant
    {
        [Key]
        public int Id { get; set; }
        public string PlantName { get; set; } = null!;
        public float? SensorValue { get; set; }
        public DateTime SensorEvent { get; set; }
    }
}
