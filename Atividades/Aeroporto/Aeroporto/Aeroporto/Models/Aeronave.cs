using System.ComponentModel.DataAnnotations;

namespace SistemaAereo.Models
{
    public class Aeronave
    {
        [Key]
        public int AeronaveId { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoAeronave { get; set; }

        [Required]
        public int NumeroPoltronas { get; set; }

        // Relacionamento com Voos
        public virtual ICollection<Voo> Voos { get; set; }

        public Aeronave()
        {
            Voos = new HashSet<Voo>();
        }
    }
}
