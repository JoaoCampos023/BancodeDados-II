﻿using System.ComponentModel.DataAnnotations;

namespace SistemaAereo.Models
{
    public class Aeroporto
    {
        [Key]
        public int AeroportoId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [StringLength(3)]
        public string CodigoIATA { get; set; }

        [StringLength(100)]
        public string Cidade { get; set; }

        [StringLength(100)]
        public string Pais { get; set; }

        // Relacionamentos
        public virtual ICollection<Voo> VoosOrigem { get; set; }
        public virtual ICollection<Voo> VoosDestino { get; set; }
        public virtual ICollection<Escala> Escalas { get; set; }

        public Aeroporto()
        {
            VoosOrigem = new HashSet<Voo>();
            VoosDestino = new HashSet<Voo>();
            Escalas = new HashSet<Escala>();
        }
    }
}
