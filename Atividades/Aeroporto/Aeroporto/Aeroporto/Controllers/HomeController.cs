using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAereo.Data;
using SistemaAereo.Models;
using SistemaAereo.Repositories;

namespace SistemaAereo.Controllers
{
    public class HomeController : Controller
    {
        private readonly AeroportoContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IClientePreferencialRepository _clienteRepository;
        private readonly IVooRepository _vooRepository;

        public HomeController(
            AeroportoContext context,
            ILogger<HomeController> logger,
            IClientePreferencialRepository clienteRepository,
            IVooRepository vooRepository)
        {
            _context = context;
            _logger = logger;
            _clienteRepository = clienteRepository;
            _vooRepository = vooRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = new DashboardViewModel
                {
                    TotalVoos = await _context.Voos.CountAsync(),
                    TotalClientes = await _clienteRepository.GetTotalClientesAtivosAsync(),
                    TotalAeronaves = await _context.Aeronaves.CountAsync(),
                    TotalAeroportos = await _context.Aeroportos.CountAsync(),
                    ProximosVoos = (await _vooRepository.GetProximosVoosAsync(5)).ToList()
                };

                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard");
                TempData["Erro"] = "Erro ao carregar dashboard";
                return View(new DashboardViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}