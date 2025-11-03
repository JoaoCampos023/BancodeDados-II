using System;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Domain.Entities
{
    public class Aviao
    {
        public int AviaoId { get; private set; }
        public string Modelo { get; private set; }
        public string Codigo { get; private set; }
        public int QuantidadePoltronas { get; private set; }
        public int QuantidadeFileiras { get; private set; }
        public int PoltronasPorFileira { get; private set; }
        public DateTime DataFabricacao { get; private set; }

        // Navigation properties
        public virtual ICollection<Voo> Voos { get; private set; }

        public Aviao() { }

        public Aviao(string modelo, string codigo, int quantidadePoltronas, int quantidadeFileiras,
                    int poltronasPorFileira, DateTime dataFabricacao)
        {
            Modelo = modelo;
            Codigo = codigo;
            QuantidadePoltronas = quantidadePoltronas;
            QuantidadeFileiras = quantidadeFileiras;
            PoltronasPorFileira = poltronasPorFileira;
            DataFabricacao = dataFabricacao;
            Voos = new HashSet<Voo>();
        }
    }
}