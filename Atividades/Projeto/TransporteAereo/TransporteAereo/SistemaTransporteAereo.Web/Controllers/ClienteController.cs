using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporteAereo.Services.Interfaces;
using SistemaTransporteAereo.Web.ViewModels;

namespace SistemaTransporteAereo.Web.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;

        public ClienteController(IClienteService clienteService, IMapper mapper)
        {
            _clienteService = clienteService;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            var clientes = _clienteService.ObterTodosClientes();
            var viewModel = _mapper.Map<ClienteViewModel>(clientes);
            return View(viewModel);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClienteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var cliente = _mapper.Map<Domain.Entities.Cliente>(viewModel);
                    _clienteService.CriarCliente(cliente);

                    TempData["Success"] = "Cliente criado com sucesso!";
                    return RedirectToAction("Index");
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(viewModel);
        }
    }
}