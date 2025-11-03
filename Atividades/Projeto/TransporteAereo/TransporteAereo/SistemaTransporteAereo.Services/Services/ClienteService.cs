using SistemaTransporteAereo.Domain.Entities;
using SistemaTransporteAereo.Domain.Interfaces;
using SistemaTransporteAereo.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Services.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public void CriarCliente(Cliente cliente)
        {
            if (_clienteRepository.Any(c => c.CPF == cliente.CPF))
                throw new InvalidOperationException("Já existe um cliente com este CPF");

            if (_clienteRepository.Any(c => c.Email == cliente.Email))
                throw new InvalidOperationException("Já existe um cliente com este email");

            _clienteRepository.Add(cliente);
        }

        public Cliente ObterClientePorId(int id)
        {
            return _clienteRepository.GetById(id);
        }

        public Cliente ObterClientePorCPF(string cpf)
        {
            return _clienteRepository.GetByCPF(cpf);
        }

        public IEnumerable<Cliente> ObterTodosClientes()
        {
            return _clienteRepository.GetAll();
        }

        public IEnumerable<Cliente> ObterClientesPreferenciais()
        {
            return _clienteRepository.GetClientesPreferenciais();
        }

        public void AtualizarCliente(Cliente cliente)
        {
            _clienteRepository.Update(cliente);
        }
    }
}