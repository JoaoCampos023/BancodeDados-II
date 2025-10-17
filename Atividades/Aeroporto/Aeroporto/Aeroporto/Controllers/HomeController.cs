// Controllers/HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IAeronaveRepository _aeronaveRepository;
        private readonly IAeroportoRepository _aeroportoRepository;
        private readonly IVooRepository _vooRepository;

        public HomeController(
            AeroportoContext context,
            ILogger<HomeController> logger,
            IClientePreferencialRepository clienteRepository,
            IAeronaveRepository aeronaveRepository,
            IAeroportoRepository aeroportoRepository,
            IVooRepository vooRepository)
        {
            _context = context;
            _logger = logger;
            _clienteRepository = clienteRepository;
            _aeronaveRepository = aeronaveRepository;
            _aeroportoRepository = aeroportoRepository;
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

        #region Clientes Preferenciais

        public async Task<IActionResult> Clientes()
        {
            try
            {
                var clientes = await _clienteRepository.GetClientesAtivosAsync();
                return View(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar clientes");
                TempData["Erro"] = "Erro ao carregar lista de clientes";
                return View(new List<ClientePreferencial>());
            }
        }

        public IActionResult CriarCliente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarCliente(ClientePreferencial cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar se email já existe
                    if (await _clienteRepository.EmailExistsAsync(cliente.Email))
                    {
                        ModelState.AddModelError("Email", "Este email já está cadastrado.");
                        return View(cliente);
                    }

                    // Verificar se CPF já existe
                    if (!string.IsNullOrEmpty(cliente.CPF) &&
                        await _clienteRepository.CPFExistsAsync(cliente.CPF))
                    {
                        ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
                        return View(cliente);
                    }

                    await _clienteRepository.AddAsync(cliente);
                    TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
                    return RedirectToAction(nameof(Clientes));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente");
                TempData["Erro"] = "Erro ao cadastrar cliente";
                return View(cliente);
            }
        }

        public async Task<IActionResult> EditarCliente(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente == null)
                {
                    TempData["Erro"] = "Cliente não encontrado";
                    return RedirectToAction(nameof(Clientes));
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar cliente para edição");
                TempData["Erro"] = "Erro ao carregar cliente";
                return RedirectToAction(nameof(Clientes));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCliente(int id, ClientePreferencial cliente)
        {
            try
            {
                if (id != cliente.ClienteId)
                {
                    TempData["Erro"] = "ID do cliente inválido";
                    return RedirectToAction(nameof(Clientes));
                }

                if (ModelState.IsValid)
                {
                    // Verificar duplicatas (excluindo o próprio registro)
                    if (await _clienteRepository.EmailExistsAsync(cliente.Email, id))
                    {
                        ModelState.AddModelError("Email", "Este email já está cadastrado.");
                        return View(cliente);
                    }

                    if (!string.IsNullOrEmpty(cliente.CPF) &&
                        await _clienteRepository.CPFExistsAsync(cliente.CPF, id))
                    {
                        ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
                        return View(cliente);
                    }

                    await _clienteRepository.UpdateAsync(cliente);
                    TempData["Sucesso"] = "Cliente atualizado com sucesso!";
                    return RedirectToAction(nameof(Clientes));
                }
                return View(cliente);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _clienteRepository.ExistsAsync(c => c.ClienteId == id))
                {
                    TempData["Erro"] = "Cliente não encontrado";
                    return RedirectToAction(nameof(Clientes));
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente");
                TempData["Erro"] = "Erro ao atualizar cliente";
                return View(cliente);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirCliente(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente != null)
                {
                    // Soft delete - marca como inativo
                    cliente.Ativo = false;
                    await _clienteRepository.UpdateAsync(cliente);
                    TempData["Sucesso"] = "Cliente excluído com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Cliente não encontrado";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente");
                TempData["Erro"] = "Erro ao excluir cliente";
            }

            return RedirectToAction(nameof(Clientes));
        }

        public async Task<IActionResult> MalaDireta()
        {
            try
            {
                var clientes = await _clienteRepository.GetClientesAtivosAsync();
                ViewBag.TotalClientes = clientes.Count();
                return View(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar mala direta");
                TempData["Erro"] = "Erro ao carregar lista de clientes";
                return View(new List<ClientePreferencial>());
            }
        }

        #endregion

        #region Aeronaves

        public async Task<IActionResult> Aeronaves()
        {
            try
            {
                var aeronaves = await _aeronaveRepository.GetAeronavesComVoosAsync();
                return View(aeronaves);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar aeronaves");
                TempData["Erro"] = "Erro ao carregar lista de aeronaves";
                return View(new List<Aeronave>());
            }
        }

        public IActionResult CriarAeronave()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarAeronave(Aeronave aeronave)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _aeronaveRepository.AddAsync(aeronave);
                    TempData["Sucesso"] = "Aeronave cadastrada com sucesso!";
                    return RedirectToAction(nameof(Aeronaves));
                }
                return View(aeronave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar aeronave");
                TempData["Erro"] = "Erro ao cadastrar aeronave";
                return View(aeronave);
            }
        }

        public async Task<IActionResult> EditarAeronave(int id)
        {
            try
            {
                var aeronave = await _aeronaveRepository.GetByIdAsync(id);
                if (aeronave == null)
                {
                    TempData["Erro"] = "Aeronave não encontrada";
                    return RedirectToAction(nameof(Aeronaves));
                }
                return View(aeronave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar aeronave para edição");
                TempData["Erro"] = "Erro ao carregar aeronave";
                return RedirectToAction(nameof(Aeronaves));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAeronave(int id, Aeronave aeronave)
        {
            try
            {
                if (id != aeronave.AeronaveId)
                {
                    TempData["Erro"] = "ID da aeronave inválido";
                    return RedirectToAction(nameof(Aeronaves));
                }

                if (ModelState.IsValid)
                {
                    await _aeronaveRepository.UpdateAsync(aeronave);
                    TempData["Sucesso"] = "Aeronave atualizada com sucesso!";
                    return RedirectToAction(nameof(Aeronaves));
                }
                return View(aeronave);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _aeronaveRepository.ExistsAsync(a => a.AeronaveId == id))
                {
                    TempData["Erro"] = "Aeronave não encontrada";
                    return RedirectToAction(nameof(Aeronaves));
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar aeronave");
                TempData["Erro"] = "Erro ao atualizar aeronave";
                return View(aeronave);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirAeronave(int id)
        {
            try
            {
                // Verificar se existem voos associados
                if (await _aeronaveRepository.HasVoosAsync(id))
                {
                    TempData["Erro"] = "Não é possível excluir a aeronave pois existem voos associados a ela.";
                    return RedirectToAction(nameof(Aeronaves));
                }

                var aeronave = await _aeronaveRepository.GetByIdAsync(id);
                if (aeronave != null)
                {
                    await _aeronaveRepository.DeleteAsync(aeronave);
                    TempData["Sucesso"] = "Aeronave excluída com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Aeronave não encontrada";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir aeronave");
                TempData["Erro"] = "Erro ao excluir aeronave";
            }

            return RedirectToAction(nameof(Aeronaves));
        }

        #endregion

        #region Aeroportos

        public async Task<IActionResult> Aeroportos()
        {
            try
            {
                var aeroportos = await _aeroportoRepository.GetAllAsync();
                return View(aeroportos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar aeroportos");
                TempData["Erro"] = "Erro ao carregar lista de aeroportos";
                return View(new List<Aeroporto>());
            }
        }

        public IActionResult CriarAeroporto()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarAeroporto(Models.Aeroporto aeroporto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar se código IATA já existe
                    if (await _aeroportoRepository.CodigoIATAExistsAsync(aeroporto.CodigoIATA))
                    {
                        ModelState.AddModelError("CodigoIATA", "Este código IATA já está cadastrado.");
                        return View(aeroporto);
                    }

                    await _aeroportoRepository.AddAsync(aeroporto);
                    TempData["Sucesso"] = "Aeroporto cadastrado com sucesso!";
                    return RedirectToAction(nameof(Aeroportos));
                }
                return View(aeroporto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar aeroporto");
                TempData["Erro"] = "Erro ao cadastrar aeroporto";
                return View(aeroporto);
            }
        }

        public async Task<IActionResult> EditarAeroporto(int id)
        {
            try
            {
                var aeroporto = await _aeroportoRepository.GetByIdAsync(id);
                if (aeroporto == null)
                {
                    TempData["Erro"] = "Aeroporto não encontrado";
                    return RedirectToAction(nameof(Aeroportos));
                }
                return View(aeroporto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar aeroporto para edição");
                TempData["Erro"] = "Erro ao carregar aeroporto";
                return RedirectToAction(nameof(Aeroportos));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAeroporto(int id, Models.Aeroporto aeroporto)
        {
            try
            {
                if (id != aeroporto.AeroportoId)
                {
                    TempData["Erro"] = "ID do aeroporto inválido";
                    return RedirectToAction(nameof(Aeroportos));
                }

                if (ModelState.IsValid)
                {
                    // Verificar duplicata de código IATA
                    if (await _aeroportoRepository.CodigoIATAExistsAsync(aeroporto.CodigoIATA, id))
                    {
                        ModelState.AddModelError("CodigoIATA", "Este código IATA já está cadastrado.");
                        return View(aeroporto);
                    }

                    await _aeroportoRepository.UpdateAsync(aeroporto);
                    TempData["Sucesso"] = "Aeroporto atualizado com sucesso!";
                    return RedirectToAction(nameof(Aeroportos));
                }
                return View(aeroporto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _aeroportoRepository.ExistsAsync(a => a.AeroportoId == id))
                {
                    TempData["Erro"] = "Aeroporto não encontrado";
                    return RedirectToAction(nameof(Aeroportos));
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar aeroporto");
                TempData["Erro"] = "Erro ao atualizar aeroporto";
                return View(aeroporto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirAeroporto(int id)
        {
            try
            {
                // Verificar se existem voos associados
                if (await _aeroportoRepository.HasVoosAsync(id))
                {
                    TempData["Erro"] = "Não é possível excluir o aeroporto pois existem voos associados a ele.";
                    return RedirectToAction(nameof(Aeroportos));
                }

                var aeroporto = await _aeroportoRepository.GetByIdAsync(id);
                if (aeroporto != null)
                {
                    await _aeroportoRepository.DeleteAsync(aeroporto);
                    TempData["Sucesso"] = "Aeroporto excluído com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Aeroporto não encontrado";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir aeroporto");
                TempData["Erro"] = "Erro ao excluir aeroporto";
            }

            return RedirectToAction(nameof(Aeroportos));
        }

        #endregion

        #region Voos

        public async Task<IActionResult> Voos()
        {
            try
            {
                var voos = await _vooRepository.GetVoosCompletosAsync();
                return View(voos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar voos");
                TempData["Erro"] = "Erro ao carregar lista de voos";
                return View(new List<Voo>());
            }
        }

        public async Task<IActionResult> CriarVoo()
        {
            try
            {
                ViewBag.Aeroportos = await _context.Aeroportos
                    .OrderBy(a => a.Nome)
                    .Select(a => new SelectListItem
                    {
                        Value = a.AeroportoId.ToString(),
                        Text = $"{a.Nome} ({a.CodigoIATA})"
                    })
                    .ToListAsync();

                ViewBag.Aeronaves = await _context.Aeronaves
                    .OrderBy(a => a.TipoAeronave)
                    .Select(a => new SelectListItem
                    {
                        Value = a.AeronaveId.ToString(),
                        Text = $"{a.TipoAeronave} - {a.NumeroPoltronas} poltronas"
                    })
                    .ToListAsync();

                return View(); // Isso procura por "CriarVoo.cshtml"
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para criar voo");
                TempData["Erro"] = "Erro ao carregar dados";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarVoo(Voo voo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar se número do voo já existe
                    if (await _vooRepository.NumeroVooExistsAsync(voo.NumeroVoo))
                    {
                        ModelState.AddModelError("NumeroVoo", "Este número de voo já está cadastrado.");
                        await CarregarViewBagsVoo();
                        return View(voo);
                    }

                    await _vooRepository.AddAsync(voo);

                    // Criar poltronas automaticamente
                    await CriarPoltronasParaVoo(voo.VooId);

                    TempData["Sucesso"] = "Voo cadastrado com sucesso!";
                    return RedirectToAction(nameof(Voos));
                }

                await CarregarViewBagsVoo();
                return View(voo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar voo");
                TempData["Erro"] = "Erro ao cadastrar voo";
                await CarregarViewBagsVoo();
                return View(voo);
            }
        }

        public async Task<IActionResult> DetalhesVoo(int id)
        {
            try
            {
                var voo = await _vooRepository.GetVooCompletoAsync(id);
                if (voo == null)
                {
                    TempData["Erro"] = "Voo não encontrado";
                    return RedirectToAction(nameof(Voos));
                }
                return View(voo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do voo");
                TempData["Erro"] = "Erro ao carregar detalhes do voo";
                return RedirectToAction(nameof(Voos));
            }
        }

        public async Task<IActionResult> EditarVoo(int id)
        {
            try
            {
                var voo = await _vooRepository.GetByIdAsync(id);
                if (voo == null)
                {
                    TempData["Erro"] = "Voo não encontrado";
                    return RedirectToAction(nameof(Voos));
                }

                await CarregarViewBagsVoo();
                return View(voo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar voo para edição");
                TempData["Erro"] = "Erro ao carregar voo";
                return RedirectToAction(nameof(Voos));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarVoo(int id, Voo voo)
        {
            try
            {
                if (id != voo.VooId)
                {
                    TempData["Erro"] = "ID do voo inválido";
                    return RedirectToAction(nameof(Voos));
                }

                if (ModelState.IsValid)
                {
                    // Verificar duplicata de número do voo
                    if (await _vooRepository.NumeroVooExistsAsync(voo.NumeroVoo, id))
                    {
                        ModelState.AddModelError("NumeroVoo", "Este número de voo já está cadastrado.");
                        await CarregarViewBagsVoo();
                        return View(voo);
                    }

                    await _vooRepository.UpdateAsync(voo);
                    TempData["Sucesso"] = "Voo atualizado com sucesso!";
                    return RedirectToAction(nameof(Voos));
                }

                await CarregarViewBagsVoo();
                return View(voo);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _vooRepository.ExistsAsync(v => v.VooId == id))
                {
                    TempData["Erro"] = "Voo não encontrado";
                    return RedirectToAction(nameof(Voos));
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar voo");
                TempData["Erro"] = "Erro ao atualizar voo";
                await CarregarViewBagsVoo();
                return View(voo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirVoo(int id)
        {
            try
            {
                var voo = await _vooRepository.GetVooCompletoAsync(id);
                if (voo != null)
                {
                    // Remover escalas e poltronas associadas
                    if (voo.Escalas.Any())
                        _context.Escalas.RemoveRange(voo.Escalas);

                    if (voo.Poltronas.Any())
                        _context.Poltronas.RemoveRange(voo.Poltronas);

                    await _vooRepository.DeleteAsync(voo);
                    TempData["Sucesso"] = "Voo excluído com sucesso!";
                }
                else
                {
                    TempData["Erro"] = "Voo não encontrado";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir voo");
                TempData["Erro"] = "Erro ao excluir voo";
            }

            return RedirectToAction(nameof(Voos));
        }

        #endregion

        #region Métodos Auxiliares

        private async Task CarregarViewBagsVoo()
        {
            ViewBag.Aeroportos = await _aeroportoRepository.GetAllAsync();
            ViewBag.Aeronaves = await _aeronaveRepository.GetAllAsync();
        }

        private async Task CriarPoltronasParaVoo(int vooId)
        {
            var voo = await _vooRepository.GetVooCompletoAsync(vooId);

            if (voo?.Aeronave != null)
            {
                var poltronas = new List<Poltrona>();
                var numeroPoltronas = voo.Aeronave.NumeroPoltronas;

                for (int i = 1; i <= numeroPoltronas; i++)
                {
                    var localizacao = (i % 2 == 0) ? "Corredor" : "Janela";
                    var tipo = i <= (numeroPoltronas * 0.05) ? "Primeira" :
                               i <= (numeroPoltronas * 0.2) ? "Executiva" : "Economica";

                    poltronas.Add(new Poltrona
                    {
                        VooId = vooId,
                        NumeroPoltrona = i.ToString("D3"),
                        Disponivel = true,
                        Localizacao = localizacao,
                        Tipo = tipo
                    });
                }

                _context.Poltronas.AddRange(poltronas);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // ViewModel para o Dashboard
    public class DashboardViewModel
    {
        public int TotalVoos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalAeronaves { get; set; }
        public int TotalAeroportos { get; set; }
        public List<Voo> ProximosVoos { get; set; } = new List<Voo>();
    }

    // ViewModel para Error
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}