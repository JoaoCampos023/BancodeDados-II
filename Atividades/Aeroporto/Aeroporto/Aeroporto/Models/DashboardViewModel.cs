using System.ComponentModel.DataAnnotations;

namespace SistemaAereo.Models
{
    public class DashboardViewModel
    {
        public int TotalVoos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalAeronaves { get; set; }
        public int TotalAeroportos { get; set; }

        // NOVAS PROPRIEDADES PARA PASSAGENS
        public int TotalPassagens { get; set; }
        public int PassagensConfirmadas { get; set; }
        public int PassagensCheckin { get; set; }
        public int PassagensEmbarcadas { get; set; }
        public int PassagensCanceladas { get; set; }
        public decimal FaturamentoTotal { get; set; }
        public decimal FaturamentoMesAtual { get; set; }

        public List<Voo> ProximosVoos { get; set; } = new List<Voo>();

        // NOVA LISTA PARA PASSAGENS RECENTES
        public List<Passagem> PassagensRecentes { get; set; } = new List<Passagem>();
    }
}