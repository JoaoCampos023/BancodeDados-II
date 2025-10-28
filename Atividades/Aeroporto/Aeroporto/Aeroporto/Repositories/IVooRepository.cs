using SistemaAereo.Models;
using System.Linq.Expressions;

namespace SistemaAereo.Repositories
{
    public interface IVooRepository : IRepository<Voo>
    {
        // CONSULTAS COMPLEXAS COM INCLUDE
        Task<IEnumerable<Voo>> GetVoosCompletosAsync();
        Task<Voo> GetVooCompletoAsync(int id);
        Task<Voo> GetVooParaEdicaoAsync(int id);

        // CONSULTAS FILTRADAS
        Task<IEnumerable<Voo>> GetProximosVoosAsync(int quantidade = 5);
        Task<IEnumerable<Voo>> GetVoosPorPeriodoAsync(DateTime inicio, DateTime fim);
        Task<IEnumerable<Voo>> GetVoosPorAeroportoAsync(int aeroportoId);
        Task<IEnumerable<Voo>> GetVoosDisponiveisAsync();

        // CONSULTAS COM FILTROS COMBINADOS
        Task<IEnumerable<Voo>> GetVoosComFiltrosAsync(
            int? aeroportoOrigemId = null,
            int? aeroportoDestinoId = null,
            int? aeronaveId = null,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            bool apenasComPoltronasDisponiveis = false);

        // VALIDAÇÕES E VERIFICAÇÕES
        Task<bool> NumeroVooExistsAsync(string numeroVoo, int? excludeId = null);
        Task<bool> HasEscalasAsync(int vooId);
        Task<bool> HasPoltronasAsync(int vooId);
        Task<bool> HasPoltronasOcupadasAsync(int vooId);

        // ESTATÍSTICAS E RELATÓRIOS
        Task<int> GetTotalVoosAsync();
        Task<int> GetTotalVoosPorAeroportoAsync(int aeroportoId);
        Task<int> GetTotalVoosPorPeriodoAsync(DateTime inicio, DateTime fim);
        Task<int> GetTotalPoltronasDisponiveisAsync(int vooId);
        Task<int> GetTotalPoltronasOcupadasAsync(int vooId);

        // CONSULTAS ESPECIALIZADAS PARA DASHBOARD
        Task<IEnumerable<Voo>> GetVoosHojeAsync();
        Task<IEnumerable<Voo>> GetVoosPorStatusAsync(string status);
        Task<Dictionary<string, int>> GetEstatisticasVoosPorAeroportoAsync();

        // OPERAÇÕES EM LOTE
        Task AtualizarStatusVoosAsync();
        Task CancelarVoosComBaixaOcupacaoAsync(double percentualMinimo);

        // CONSULTAS PAGINADAS (PARA GRANDES VOLUMES DE DADOS)
        Task<(IEnumerable<Voo> Voos, int TotalCount)> GetVoosPaginadosAsync(
            int pagina = 1,
            int itensPorPagina = 10,
            string ordenacao = "data",
            bool ascendente = true);
    }

    // CLASSE DE SUPORTE PARA FILTROS COMPLEXOS
    public class VooFiltros
    {
        public int? AeroportoOrigemId { get; set; }
        public int? AeroportoDestinoId { get; set; }
        public int? AeronaveId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool ApenasComPoltronasDisponiveis { get; set; }
        public string NumeroVoo { get; set; }
        public bool ApenasFuturos { get; set; } = true;

        // Paginação
        public int Pagina { get; set; } = 1;
        public int ItensPorPagina { get; set; } = 10;

        // Ordenação
        public string Ordenacao { get; set; } = "data";
        public bool Ascendente { get; set; } = true;
    }

    // CLASSE DE RESPOSTA PAGINADA
    public class ResultadoPaginado<T>
    {
        public IEnumerable<T> Itens { get; set; }
        public int Pagina { get; set; }
        public int ItensPorPagina { get; set; }
        public int TotalItens { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / ItensPorPagina);
        public bool TemPaginaAnterior => Pagina > 1;
        public bool TemProximaPagina => Pagina < TotalPaginas;
    }
}