using System.ComponentModel.DataAnnotations;

namespace SistemaAereo.Models
{
    public class Voo
    {
        [Key]
        public int VooId { get; set; }

        [Required]
        [StringLength(10)]
        public string NumeroVoo { get; set; }

        [Required]
        public int AeroportoOrigemId { get; set; }

        [Required]
        public int AeroportoDestinoId { get; set; }

        [Required]
        public int AeronaveId { get; set; }

        [Required]
        public DateTime HorarioSaida { get; set; }

        [Required]
        public DateTime HorarioChegadaPrevisto { get; set; }

        // Relacionamentos
        public virtual Aeroporto AeroportoOrigem { get; set; }
        public virtual Aeroporto AeroportoDestino { get; set; }
        public virtual Aeronave Aeronave { get; set; }
        public virtual ICollection<Escala> Escalas { get; set; }
        public virtual ICollection<Poltrona> Poltronas { get; set; }

        public Voo()
        {
            Escalas = new HashSet<Escala>();
            Poltronas = new HashSet<Poltrona>();
        }
    }
}
