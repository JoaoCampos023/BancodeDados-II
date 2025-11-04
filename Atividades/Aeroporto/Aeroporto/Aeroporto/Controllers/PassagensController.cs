using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAereo.Data;
using SistemaAereo.Models;
using SistemaAereo.Repositories;
using SistemaAereo.Repositories.Interfaces;

namespace SistemaAereo.Controllers
{
    public class PassagensController : Controller
    {
        private readonly IPassagemRepository _passagemRepository;
        private readonly IPoltronaRepository _poltronaRepository;
        private readonly IClientePreferencialRepository _clienteRepository;
        private readonly IVooRepository _vooRepository;
        private readonly AeroportoContext _context;
        private readonly ILogger<PassagensController> _logger;

        public PassagensController(
            IPassagemRepository passagemRepository,
            IPoltronaRepository poltronaRepository,
            IClientePreferencialRepository clienteRepository,
            IVooRepository vooRepository,
            AeroportoContext context,
            ILogger<PassagensController> logger)
        {
            _passagemRepository = passagemRepository;
            _poltronaRepository = poltronaRepository;
            _clienteRepository = clienteRepository;
            _vooRepository = vooRepository;
            _context = context;
            _logger = logger;
        }

        // GET: Passagens
        public async Task<IActionResult> Index()
        {
            try
            {
                var passagens = await _passagemRepository.GetPassagensCompletasAsync();
                return View(passagens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar passagens");
                TempData["Erro"] = "Erro ao carregar lista de passagens";
                return View(new List<Passagem>());
            }
        }

        // GET: Passagens/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                _logger.LogInformation("=== ACESSANDO Passagens/Create GET ===");
                await CarregarViewBags();
                _logger.LogInformation("Página Create carregada com sucesso");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERRO ao carregar página Create");
                TempData["Erro"] = "Erro ao carregar formulário de passagem";
                return View();
            }
        }

        // POST: Passagens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Passagem passagem)
        {
            try
            {
                _logger.LogInformation("=== TENTATIVA DE CRIAÇÃO DE PASSAGEM ===");

                // Limpar o ModelState e validar manualmente
                ModelState.Clear();

                // Validação manual dos campos
                if (passagem.ClienteId <= 0)
                    ModelState.AddModelError("ClienteId", "Cliente é obrigatório.");

                if (passagem.VooId <= 0)
                    ModelState.AddModelError("VooId", "Voo é obrigatório.");

                if (passagem.PoltronaId <= 0)
                    ModelState.AddModelError("PoltronaId", "Poltrona é obrigatória.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Validação manual falhou");
                    await CarregarViewBags();
                    return View(passagem);
                }

                _logger.LogInformation($"Dados válidos - Cliente: {passagem.ClienteId}, Voo: {passagem.VooId}, Poltrona: {passagem.PoltronaId}");

                // Buscar poltrona
                var poltrona = await _context.Poltronas
                    .FirstOrDefaultAsync(p => p.PoltronaId == passagem.PoltronaId && p.Disponivel);

                if (poltrona == null)
                {
                    ModelState.AddModelError("PoltronaId", "Poltrona não encontrada ou indisponível.");
                    await CarregarViewBags();
                    return View(passagem);
                }

                // Buscar cliente
                var cliente = await _context.ClientesPreferenciais
                    .FirstOrDefaultAsync(c => c.ClienteId == passagem.ClienteId && c.Ativo);

                if (cliente == null)
                {
                    ModelState.AddModelError("ClienteId", "Cliente não encontrado.");
                    await CarregarViewBags();
                    return View(passagem);
                }

                // Buscar voo
                var voo = await _context.Voos
                    .FirstOrDefaultAsync(v => v.VooId == passagem.VooId);

                if (voo == null)
                {
                    ModelState.AddModelError("VooId", "Voo não encontrado.");
                    await CarregarViewBags();
                    return View(passagem);
                }

                // Verificar se poltrona já está ocupada
                var poltronaOcupada = await _context.Passagens
                    .AnyAsync(p => p.PoltronaId == passagem.PoltronaId && p.Status != "Cancelada");

                if (poltronaOcupada)
                {
                    ModelState.AddModelError("PoltronaId", "Poltrona já ocupada.");
                    await CarregarViewBags();
                    return View(passagem);
                }

                // Preencher dados automáticos
                passagem.NumeroBilhete = GerarNumeroBilhete();
                passagem.DataEmissao = DateTime.Now;
                passagem.Status = "Confirmada";
                passagem.Classe = poltrona.Tipo;
                passagem.Preco = poltrona.Preco;

                // Salvar passagem
                _context.Passagens.Add(passagem);
                await _context.SaveChangesAsync();

                // Atualizar poltrona
                poltrona.Disponivel = false;
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = $"Passagem emitida com sucesso! Número: {passagem.NumeroBilhete}";
                return RedirectToAction(nameof(Details), new { id = passagem.PassagemId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar passagem");
                TempData["Erro"] = "Erro ao criar passagem: " + ex.Message;
                await CarregarViewBags();
                return View(passagem);
            }
        }

        // GET: Passagens/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var passagem = await _passagemRepository.GetPassagemCompletaAsync(id);
                if (passagem == null)
                {
                    TempData["Erro"] = "Passagem não encontrada";
                    return RedirectToAction(nameof(Index));
                }
                return View(passagem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da passagem {PassagemId}", id);
                TempData["Erro"] = "Erro ao carregar passagem";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Passagens/Checkin/5
        public async Task<IActionResult> Checkin(int id)
        {
            try
            {
                var passagem = await _passagemRepository.GetByIdAsync(id);
                if (passagem == null)
                {
                    TempData["Erro"] = "Passagem não encontrada";
                    return RedirectToAction(nameof(Index));
                }

                if (passagem.Status != "Confirmada")
                {
                    TempData["Erro"] = $"Check-in não permitido. Status atual: {passagem.Status}";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                passagem.Status = "Check-in";
                await _passagemRepository.UpdateAsync(passagem);

                TempData["Sucesso"] = "Check-in realizado com sucesso!";
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar check-in da passagem {PassagemId}", id);
                TempData["Erro"] = "Erro ao realizar check-in";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Passagens/Embarque/5
        public async Task<IActionResult> Embarque(int id)
        {
            try
            {
                var passagem = await _passagemRepository.GetByIdAsync(id);
                if (passagem == null)
                {
                    TempData["Erro"] = "Passagem não encontrada";
                    return RedirectToAction(nameof(Index));
                }

                if (passagem.Status != "Check-in")
                {
                    TempData["Erro"] = $"Embarque não permitido. Status atual: {passagem.Status}. É necessário fazer check-in primeiro.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                passagem.Status = "Embarcada";
                await _passagemRepository.UpdateAsync(passagem);

                TempData["Sucesso"] = "Embarque registrado com sucesso!";
                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar embarque da passagem {PassagemId}", id);
                TempData["Erro"] = "Erro ao registrar embarque";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Passagens/Cancelar/5
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var passagem = await _passagemRepository.GetPassagemCompletaAsync(id);
                if (passagem == null)
                {
                    TempData["Erro"] = "Passagem não encontrada";
                    return RedirectToAction(nameof(Index));
                }

                if (passagem.Status == "Cancelada")
                {
                    TempData["Info"] = "Esta passagem já está cancelada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                passagem.Status = "Cancelada";
                await _passagemRepository.UpdateAsync(passagem);

                // Liberar poltrona
                var poltrona = await _poltronaRepository.GetByIdAsync(passagem.PoltronaId);
                if (poltrona != null)
                {
                    poltrona.Disponivel = true;
                    await _poltronaRepository.UpdateAsync(poltrona);
                }

                TempData["Sucesso"] = "Passagem cancelada com sucesso! A poltrona foi liberada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar passagem {PassagemId}", id);
                TempData["Erro"] = "Erro ao cancelar passagem";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Passagens/PoltronasDisponiveis
        public async Task<JsonResult> PoltronasDisponiveis(int vooId)
        {
            try
            {
                _logger.LogInformation($"Buscando poltronas para voo {vooId}");

                if (vooId <= 0)
                {
                    return Json(new { success = false, message = "ID do voo inválido" });
                }

                // Buscar poltronas disponíveis para o voo
                var poltronas = await _context.Poltronas
                    .Where(p => p.VooId == vooId && p.Disponivel)
                    .OrderBy(p => p.NumeroPoltrona)
                    .ToListAsync();

                _logger.LogInformation($"Encontradas {poltronas.Count} poltronas disponíveis para voo {vooId}");

                var result = poltronas.Select(p => new
                {
                    poltronaId = p.PoltronaId,
                    numeroPoltrona = p.NumeroPoltrona,
                    tipo = p.Tipo,
                    localizacao = p.Localizacao,
                    preco = p.Preco
                }).ToList();

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar poltronas para voo {VooId}", vooId);
                return Json(new { success = false, message = "Erro ao carregar poltronas" });
            }
        }

        // GET: Passagens/PorCliente/5
        public async Task<IActionResult> PorCliente(int id)
        {
            try
            {
                var passagens = await _passagemRepository.GetPassagensPorClienteAsync(id);
                var cliente = await _clienteRepository.GetByIdAsync(id);

                if (cliente != null)
                {
                    ViewBag.ClienteNome = cliente.Nome;
                }

                return View(passagens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar passagens do cliente {ClienteId}", id);
                TempData["Erro"] = "Erro ao carregar passagens do cliente";
                return View(new List<Passagem>());
            }
        }

        // GET: Passagens/PorVoo/5
        public async Task<IActionResult> PorVoo(int id)
        {
            try
            {
                var passagens = await _passagemRepository.GetPassagensPorVooAsync(id);
                var voo = await _vooRepository.GetByIdAsync(id);

                if (voo != null)
                {
                    ViewBag.VooNumero = voo.NumeroVoo;
                }

                return View(passagens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar passagens do voo {VooId}", id);
                TempData["Erro"] = "Erro ao carregar passagens do voo";
                return View(new List<Passagem>());
            }
        }

        private async Task CarregarViewBags()
        {
            try
            {
                _logger.LogInformation("=== INICIANDO CarregarViewBags ===");

                // 1. Carregar clientes - com verificação robusta
                var clientesAtivos = await _context.ClientesPreferenciais
                    .Where(c => c.Ativo)
                    .OrderBy(c => c.Nome)
                    .ToListAsync();

                _logger.LogInformation($"Clientes ativos encontrados: {clientesAtivos.Count}");

                if (clientesAtivos.Any())
                {
                    ViewBag.Clientes = new SelectList(clientesAtivos, "ClienteId", "Nome");
                    _logger.LogInformation("ViewBag.Clientes carregado com sucesso");
                }
                else
                {
                    ViewBag.Clientes = new SelectList(new List<ClientePreferencial>(), "ClienteId", "Nome");
                    _logger.LogWarning("Nenhum cliente ativo encontrado");
                }

                // 2. Carregar voos - com verificação robusta
                var agora = DateTime.Now;
                var voosDisponiveis = await _context.Voos
                    .Include(v => v.AeroportoOrigem)
                    .Include(v => v.AeroportoDestino)
                    .Include(v => v.Poltronas)
                    .Where(v => v.HorarioSaida > agora && v.Poltronas.Any(p => p.Disponivel))
                    .OrderBy(v => v.HorarioSaida)
                    .ToListAsync();

                _logger.LogInformation($"Voos disponíveis encontrados: {voosDisponiveis.Count}");

                if (voosDisponiveis.Any())
                {
                    var voosSelectList = voosDisponiveis.Select(v => new
                    {
                        VooId = v.VooId,
                        DisplayText = $"{v.NumeroVoo} - {v.AeroportoOrigem?.CodigoIATA ?? "N/A"} → {v.AeroportoDestino?.CodigoIATA ?? "N/A"} - {v.HorarioSaida:dd/MM/yyyy HH:mm}"
                    }).ToList();

                    ViewBag.VoosDetalhados = new SelectList(voosSelectList, "VooId", "DisplayText");
                    _logger.LogInformation("ViewBag.VoosDetalhados carregado com sucesso");
                }
                else
                {
                    ViewBag.VoosDetalhados = new SelectList(new List<object>(), "VooId", "DisplayText");
                    _logger.LogWarning("Nenhum voo disponível encontrado");
                }

                _logger.LogInformation("=== CarregarViewBags FINALIZADO ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERRO CRÍTICO em CarregarViewBags");

                // Garantir que os ViewBags nunca sejam null
                ViewBag.Clientes = new SelectList(new List<ClientePreferencial>(), "ClienteId", "Nome");
                ViewBag.VoosDetalhados = new SelectList(new List<object>(), "VooId", "DisplayText");

                _logger.LogInformation("ViewBags definidos com valores padrão devido ao erro");
            }
        }

        // No PassagensController - adicione este método
        public async Task<IActionResult> VerificarDados()
        {
            try
            {
                var dados = new
                {
                    ClientesCount = await _context.ClientesPreferenciais.CountAsync(c => c.Ativo),
                    VoosCount = await _context.Voos.CountAsync(v => v.HorarioSaida > DateTime.Now),
                    VoosComPoltronasCount = await _context.Voos
                        .Include(v => v.Poltronas)
                        .CountAsync(v => v.HorarioSaida > DateTime.Now && v.Poltronas.Any(p => p.Disponivel)),
                    PoltronasDisponiveisCount = await _context.Poltronas.CountAsync(p => p.Disponivel)
                };

                return Json(new
                {
                    success = true,
                    message = "Dados verificados com sucesso",
                    data = dados
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // No PassagensController - adicione este método
        public async Task<IActionResult> DebugDadosCompleto()
        {
            try
            {
                // Verificar clientes
                var clientes = await _context.ClientesPreferenciais.ToListAsync();
                var clientesAtivos = await _context.ClientesPreferenciais
                    .Where(c => c.Ativo)
                    .ToListAsync();

                // Verificar voos
                var voos = await _context.Voos.ToListAsync();
                var voosComPoltronas = await _context.Voos
                    .Include(v => v.Poltronas)
                    .Where(v => v.HorarioSaida > DateTime.Now && v.Poltronas.Any(p => p.Disponivel))
                    .ToListAsync();

                // Verificar poltronas
                var poltronas = await _context.Poltronas.ToListAsync();
                var poltronasDisponiveis = await _context.Poltronas
                    .Where(p => p.Disponivel)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    clientes = new
                    {
                        total = clientes.Count,
                        ativos = clientesAtivos.Count,
                        dados = clientesAtivos.Select(c => new { c.ClienteId, c.Nome, c.Ativo })
                    },
                    voos = new
                    {
                        total = voos.Count,
                        comPoltronasDisponiveis = voosComPoltronas.Count,
                        dados = voosComPoltronas.Select(v => new {
                            v.VooId,
                            v.NumeroVoo,
                            origem = v.AeroportoOrigem?.CodigoIATA,
                            destino = v.AeroportoDestino?.CodigoIATA,
                            saida = v.HorarioSaida,
                            poltronasDisponiveis = v.Poltronas.Count(p => p.Disponivel)
                        })
                    },
                    poltronas = new
                    {
                        total = poltronas.Count,
                        disponiveis = poltronasDisponiveis.Count
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        public async Task<IActionResult> DebugVoos()
        {
            try
            {
                // Verificar TODOS os voos e seu status
                var todosVoos = await _context.Voos
                    .Include(v => v.AeroportoOrigem)
                    .Include(v => v.AeroportoDestino)
                    .Include(v => v.Aeronave)
                    .Include(v => v.Poltronas)
                    .OrderBy(v => v.HorarioSaida)
                    .ToListAsync();

                var resultado = new
                {
                    success = true,
                    totalVoos = todosVoos.Count,
                    voos = todosVoos.Select(v => new
                    {
                        v.VooId,
                        v.NumeroVoo,
                        origem = v.AeroportoOrigem?.CodigoIATA ?? "N/A",
                        destino = v.AeroportoDestino?.CodigoIATA ?? "N/A",
                        saida = v.HorarioSaida.ToString("dd/MM/yyyy HH:mm"),
                        ehFuturo = v.HorarioSaida > DateTime.Now,
                        totalPoltronas = v.Poltronas.Count,
                        poltronasDisponiveis = v.Poltronas.Count(p => p.Disponivel),
                        temAeronave = v.Aeronave != null,
                        temOrigem = v.AeroportoOrigem != null,
                        temDestino = v.AeroportoDestino != null,
                        status = v.HorarioSaida > DateTime.Now ?
                            (v.Poltronas.Any(p => p.Disponivel) ? "DISPONÍVEL" : "SEM POLTRONAS") :
                            "PASSADO"
                    })
                };

                return Json(resultado);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public async Task<IActionResult> CriarPoltronasParaVoos()
        {
            try
            {
                // Buscar todos os voos futuros sem poltronas
                var voosSemPoltronas = await _context.Voos
                    .Include(v => v.Aeronave)
                    .Include(v => v.Poltronas)
                    .Where(v => v.HorarioSaida > DateTime.Now && !v.Poltronas.Any())
                    .ToListAsync();

                _logger.LogInformation($"Encontrados {voosSemPoltronas.Count} voos sem poltronas");

                var poltronasCriadas = 0;

                foreach (var voo in voosSemPoltronas)
                {
                    // Verificar se o voo tem aeronave com número de poltronas
                    if (voo.Aeronave != null && voo.Aeronave.NumeroPoltronas > 0)
                    {
                        await CriarPoltronasParaVoo(voo.VooId, voo.Aeronave.NumeroPoltronas);
                        poltronasCriadas++;
                    }
                    else
                    {
                        // Se não tem aeronave ou número de poltronas, criar um número padrão
                        await CriarPoltronasParaVoo(voo.VooId, 50); // 50 poltronas padrão
                        poltronasCriadas++;
                    }
                }

                return Json(new
                {
                    success = true,
                    message = $"Poltronas criadas para {poltronasCriadas} voos",
                    voosProcessados = voosSemPoltronas.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar poltronas para voos");
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Método auxiliar para criar poltronas para um voo específico
        private async Task CriarPoltronasParaVoo(int vooId, int numeroPoltronas)
        {
            var voo = await _context.Voos.FindAsync(vooId);
            if (voo == null) return;

            var poltronas = new List<Poltrona>();
            var tipos = new[] { "Economica", "Executiva" };
            var localizacoes = new[] { "Janela", "Corredor", "Meio" };
            var random = new Random();

            for (int i = 1; i <= numeroPoltronas; i++)
            {
                var fileira = (i - 1) / 6 + 1; // 6 poltronas por fileira
                var assento = (i - 1) % 6 + 1;
                var letraAssento = ((char)('A' + (assento - 1))).ToString();

                var tipo = i <= numeroPoltronas * 0.2 ? "Executiva" : "Economica"; // 20% executiva
                var localizacao = assento switch
                {
                    1 or 6 => "Janela",
                    2 or 5 => "Meio",
                    3 or 4 => "Corredor",
                    _ => "Corredor"
                };

                var precoBase = tipo == "Executiva" ? 500.00m : 300.00m;
                var preco = precoBase + (random.Next(-50, 51)); // Variação de +/- R$50

                var poltrona = new Poltrona
                {
                    VooId = vooId,
                    NumeroPoltrona = $"{fileira}{letraAssento}",
                    Disponivel = true,
                    Localizacao = localizacao,
                    Tipo = tipo,
                    Preco = preco
                };

                poltronas.Add(poltrona);
            }

            await _context.Poltronas.AddRangeAsync(poltronas);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Criadas {poltronas.Count} poltronas para o voo {voo.NumeroVoo}");
        }

        public async Task<IActionResult> CriarPoltronasParaVoo(int vooId, int? numeroPoltronas = null)
        {
            try
            {
                var voo = await _context.Voos
                    .Include(v => v.Aeronave)
                    .Include(v => v.Poltronas)
                    .FirstOrDefaultAsync(v => v.VooId == vooId);

                if (voo == null)
                {
                    return Json(new { success = false, message = "Voo não encontrado" });
                }

                // Verificar se já tem poltronas
                if (voo.Poltronas.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Voo já possui {voo.Poltronas.Count} poltronas cadastradas"
                    });
                }

                // Determinar número de poltronas
                int numPoltronas = numeroPoltronas ?? voo.Aeronave?.NumeroPoltronas ?? 50;

                await CriarPoltronasParaVoo(vooId, numPoltronas);

                return Json(new
                {
                    success = true,
                    message = $"Criadas {numPoltronas} poltronas para o voo {voo.NumeroVoo}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar poltronas para voo {vooId}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        private string GerarNumeroBilhete()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"{timestamp}{random}";
        }

        // Método para debug - verificar dados disponíveis
        public async Task<JsonResult> DebugDados()
        {
            try
            {
                var clientesCount = await _clienteRepository.GetTotalClientesAtivosAsync();
                var voosCount = await _context.Voos
                    .Where(v => v.HorarioSaida > DateTime.Now &&
                               v.Poltronas.Any(p => p.Disponivel))
                    .CountAsync();

                var poltronasCount = await _context.Poltronas
                    .Where(p => p.Disponivel && p.Voo.HorarioSaida > DateTime.Now)
                    .CountAsync();

                return Json(new
                {
                    clientesAtivos = clientesCount,
                    voosDisponiveis = voosCount,
                    poltronasDisponiveis = poltronasCount,
                    timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}