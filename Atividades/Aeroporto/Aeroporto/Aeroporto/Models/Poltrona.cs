using System.ComponentModel.DataAnnotations;

namespace SistemaAereo.Models
{
    public class Poltrona
    {
        [Key]
        public int PoltronaId { get; set; }

        [Required]
        public int VooId { get; set; }

        [Required]
        [StringLength(10)]
        public string NumeroPoltrona { get; set; }

        [Required]
        public bool Disponivel { get; set; } = true;

        [Required]
        [StringLength(20)]
        public string Localizacao { get; set; } // "Janela", "Corredor", "Meio"

        [StringLength(20)]
        public string Tipo { get; set; } // "Economica", "Executiva", "Primeira"

        [DataType(DataType.Currency)]
        public decimal Preco { get; set; }

        // Relacionamento com Passagem
        public virtual ICollection<Passagem> Passagens { get; set; }

        // Relacionamento com Voo
        public virtual Voo Voo { get; set; }

        public Poltrona()
        {
            Passagens = new HashSet<Passagem>();
        }
    }
}