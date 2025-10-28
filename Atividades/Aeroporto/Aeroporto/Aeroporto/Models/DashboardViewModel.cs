using SistemaAereo.Models;

namespace SistemaAereo.Models
{
    public class DashboardViewModel
    {
        public int TotalVoos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalAeronaves { get; set; }
        public int TotalAeroportos { get; set; }
        public List<Voo> ProximosVoos { get; set; } = new List<Voo>();
    }
}