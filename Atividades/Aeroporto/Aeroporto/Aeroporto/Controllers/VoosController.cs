// Controllers/VoosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAereo.Data;
using SistemaAereo.Models;
using SistemaAereo.Repositories;

namespace SistemaAereo.Controllers
{
    public class VoosController : Controller
    {
        private readonly AeroportoContext _context;
        private readonly IVooRepository _vooRepository;
        private readonly IAeroportoRepository _aeroportoRepository;
        private readonly IAeronaveRepository _aeronaveRepository;
        private readonly ILogger<VoosController> _logger;

        public VoosController(
            AeroportoContext context,
            IVooRepository vooRepository,
            IAeroportoRepository aeroportoRepository,
            IAeronaveRepository aeronaveRepository,
            ILogger<VoosController> logger)
        {
            _context = context;
            _vooRepository = vooRepository;
            _aeroportoRepository = aeroportoRepository;
            _aeronaveRepository = aeronaveRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
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

        public async Task<IActionResult> Create()
        {
            await CarregarViewBags();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voo voo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _vooRepository.NumeroVooExistsAsync(voo.NumeroVoo))
                    {
                        ModelState.AddModelError("NumeroVoo", "Este número de voo já está cadastrado.");
                        await CarregarViewBags();
                        return View(voo);
                    }

                    await _vooRepository.AddAsync(voo);
                    await CriarPoltronasParaVoo(voo.VooId);

                    TempData["Sucesso"] = "Voo cadastrado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                await CarregarViewBags();
                return View(voo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar voo");
                TempData["Erro"] = "Erro ao cadastrar voo";
                await CarregarViewBags();
                return View(voo);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var voo = await _vooRepository.GetVooCompletoAsync(id);
                if (voo == null)
                {
                    TempData["Erro"] = "Voo não encontrado";
                    return RedirectToAction(nameof(Index));
                }
                return View(voo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do voo");
                TempData["Erro"] = "Erro ao carregar detalhes do voo";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var voo = await _vooRepository.GetByIdAsync(id);
                if (voo == null)
                {
                    TempData["Erro"] = "Voo não encontrado";
                    return RedirectToAction(nameof(Index));
                }

                await CarregarViewBags();
                return View(voo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar voo para edição");
                TempData["Erro"] = "Erro ao carregar voo";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Voo voo)
        {
            try
            {
                if (id != voo.VooId)
                {
                    TempData["Erro"] = "ID do voo inválido";
                    return RedirectToAction(nameof(Index));
                }

                if (ModelState.IsValid)
                {
                    if (await _vooRepository.NumeroVooExistsAsync(voo.NumeroVoo, id))
                    {
                        ModelState.AddModelError("NumeroVoo", "Este número de voo já está cadastrado.");
                        await CarregarViewBags();
                        return View(voo);
                    }

                    await _vooRepository.UpdateAsync(voo);
                    TempData["Sucesso"] = "Voo atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                await CarregarViewBags();
                return View(voo);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _vooRepository.ExistsAsync(v => v.VooId == id))
                {
                    TempData["Erro"] = "Voo não encontrado";
                    return RedirectToAction(nameof(Index));
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar voo");
                TempData["Erro"] = "Erro ao atualizar voo";
                await CarregarViewBags();
                return View(voo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var voo = await _vooRepository.GetVooCompletoAsync(id);
                if (voo != null)
                {
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

            return RedirectToAction(nameof(Index));
        }

        private async Task CarregarViewBags()
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar ViewBags para Voo");
                ViewBag.Aeroportos = new List<SelectListItem>();
                ViewBag.Aeronaves = new List<SelectListItem>();
            }
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
    }
}