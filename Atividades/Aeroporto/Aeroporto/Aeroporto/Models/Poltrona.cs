using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Display(Name = "Número da Poltrona")]
        public string NumeroPoltrona { get; set; }

        [Required]
        [Display(Name = "Disponível")]
        public bool Disponivel { get; set; } = true;

        [StringLength(20)]
        [Display(Name = "Localização")]
        public string Localizacao { get; set; } // "Janela", "Corredor", "Meio"

        [StringLength(20)]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; } // "Economica", "Executiva", "Primeira"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Preço")]
        public decimal Preco { get; set; }

        // Relacionamentos
        [ForeignKey("VooId")]
        public virtual Voo Voo { get; set; }

        public Poltrona()
        {
        }
    }
}