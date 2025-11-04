using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAereo.Data;
using SistemaAereo.Models;

namespace SistemaAereo.Controllers
{
    public class VoosController : Controller
    {
        private readonly AeroportoContext _context;

        public VoosController(AeroportoContext context)
        {
            _context = context;
        }

        // =============================================
        // MÉTODOS PRINCIPAIS - CRUD
        // =============================================

        // GET: Voos
        public async Task<IActionResult> Index()
        {
            var voos = await _context.Voos
                .Include(v => v.AeroportoOrigem)
                .Include(v => v.AeroportoDestino)
                .Include(v => v.Aeronave)
                .OrderBy(v => v.HorarioSaida)
                .ToListAsync();

            return View(voos);
        }

        // GET: Voos/Create
        public async Task<IActionResult> Create()
        {
            await CarregarViewBags();
            return View(new Voo());
        }

        // POST: Voos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string NumeroVoo,
            int AeroportoOrigemId,
            int AeroportoDestinoId,
            int AeronaveId,
            DateTime HorarioSaida,
            DateTime HorarioChegadaPrevisto)
        {
            try
            {
                Console.WriteLine("=== DADOS RECEBIDOS VIA PARÂMETROS ===");
                Console.WriteLine($"NumeroVoo: {NumeroVoo}");
                Console.WriteLine($"AeroportoOrigemId: {AeroportoOrigemId}");
                Console.WriteLine($"AeroportoDestinoId: {AeroportoDestinoId}");
                Console.WriteLine($"AeronaveId: {AeronaveId}");
                Console.WriteLine($"HorarioSaida: {HorarioSaida}");
                Console.WriteLine($"HorarioChegadaPrevisto: {HorarioChegadaPrevisto}");

                var voo = new Voo
                {
                    NumeroVoo = NumeroVoo?.Trim().ToUpper(),
                    AeroportoOrigemId = AeroportoOrigemId,
                    AeroportoDestinoId = AeroportoDestinoId,
                    AeronaveId = AeronaveId,
                    HorarioSaida = HorarioSaida,
                    HorarioChegadaPrevisto = HorarioChegadaPrevisto
                };

                ValidarVoo(voo);

                if (ModelState.IsValid)
                {
                    _context.Voos.Add(voo);
                    await _context.SaveChangesAsync();

                    TempData["Sucesso"] = $"Voo {voo.NumeroVoo} cadastrado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }

                await CarregarViewBags();
                return View(voo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO: {ex.Message}");
                TempData["Erro"] = $"Erro: {ex.Message}";
                await CarregarViewBags();

                var voo = new Voo
                {
                    NumeroVoo = NumeroVoo,
                    AeroportoOrigemId = AeroportoOrigemId,
                    AeroportoDestinoId = AeroportoDestinoId,
                    AeronaveId = AeronaveId,
                    HorarioSaida = HorarioSaida,
                    HorarioChegadaPrevisto = HorarioChegadaPrevisto
                };

                return View(voo);
            }
        }

        // GET: Voos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var voo = await _context.Voos
                .Include(v => v.AeroportoOrigem)
                .Include(v => v.AeroportoDestino)
                .Include(v => v.Aeronave)
                .FirstOrDefaultAsync(v => v.VooId == id);

            if (voo == null) return RedirectToAction(nameof(Index));

            return View(voo);
        }

        // GET: Voos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var voo = await _context.Voos.FindAsync(id);
            if (voo == null) return RedirectToAction(nameof(Index));

            await CarregarViewBags();
            return View(voo);
        }

        // POST: Voos/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Voo voo)
        {
            if (id != voo.VooId) return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                _context.Update(voo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await CarregarViewBags();
            return View(voo);
        }

        // GET: Voos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var voo = await _context.Voos
                .Include(v => v.AeroportoOrigem)
                .Include(v => v.AeroportoDestino)
                .Include(v => v.Aeronave)
                .FirstOrDefaultAsync(v => v.VooId == id);

            if (voo == null) return RedirectToAction(nameof(Index));

            return View(voo);
        }

        // POST: Voos/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voo = await _context.Voos.FindAsync(id);
            if (voo != null)
            {
                _context.Voos.Remove(voo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // =============================================
        // MÉTODOS PRIVADOS AUXILIARES
        // =============================================

        private async Task CarregarViewBags()
        {
            var aeroportos = await _context.Aeroportos
                .OrderBy(a => a.Nome)
                .ToListAsync();

            ViewBag.Aeroportos = aeroportos
                .Select(a => new SelectListItem
                {
                    Value = a.AeroportoId.ToString(),
                    Text = $"{a.Nome} ({a.CodigoIATA}) - {a.Cidade}"
                })
                .ToList();

            var aeronaves = await _context.Aeronaves
                .OrderBy(a => a.TipoAeronave)
                .ToListAsync();

            ViewBag.Aeronaves = aeronaves
                .Select(a => new SelectListItem
                {
                    Value = a.AeronaveId.ToString(),
                    Text = $"{a.TipoAeronave} - {a.NumeroPoltronas} poltronas"
                })
                .ToList();
        }

        private async void ValidarVoo(Voo voo)
        {
            if (voo.AeroportoOrigemId == voo.AeroportoDestinoId)
            {
                ModelState.AddModelError("AeroportoDestinoId", "O aeroporto de destino deve ser diferente do aeroporto de origem.");
            }

            if (voo.HorarioChegadaPrevisto <= voo.HorarioSaida)
            {
                ModelState.AddModelError("HorarioChegadaPrevisto", "O horário de chegada deve ser posterior ao horário de saída.");
            }

            if (await _context.Voos.AnyAsync(v => v.NumeroVoo == voo.NumeroVoo))
            {
                ModelState.AddModelError("NumeroVoo", "Este número de voo já está cadastrado.");
            }
        }
    }
}