using System.Collections.Generic;

namespace SistemaTransporteAereo.Domain.Entities
{
    public class Aeroporto
    {
        public int AeroportoId { get; private set; }
        public string Codigo { get; private set; }
        public string Nome { get; private set; }
        public string Cidade { get; private set; }
        public string Pais { get; private set; }

        // Navigation properties
        public virtual ICollection<Voo> VoosOrigem { get; private set; }
        public virtual ICollection<Voo> VoosDestino { get; private set; }

        public Aeroporto() { }

        public Aeroporto(string codigo, string nome, string cidade, string pais)
        {
            Codigo = codigo;
            Nome = nome;
            Cidade = cidade;
            Pais = pais;
            VoosOrigem = new HashSet<Voo>();
            VoosDestino = new HashSet<Voo>();
        }
    }
}