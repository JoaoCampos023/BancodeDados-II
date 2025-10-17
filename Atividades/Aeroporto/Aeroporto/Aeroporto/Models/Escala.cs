using System.ComponentModel.DataAnnotations;

namespace SistemaAereo.Models
{
    public class Escala
    {
        [Key]
        public int EscalaId { get; set; }

        [Required]
        public int VooId { get; set; }

        [Required]
        public int AeroportoId { get; set; }

        [Required]
        public int Ordem { get; set; }

        [Required]
        public DateTime HorarioSaida { get; set; }

        public DateTime? HorarioChegada { get; set; }

        // Relacionamentos
        public virtual Voo Voo { get; set; }
        public virtual Aeroporto Aeroporto { get; set; }
    }
}
