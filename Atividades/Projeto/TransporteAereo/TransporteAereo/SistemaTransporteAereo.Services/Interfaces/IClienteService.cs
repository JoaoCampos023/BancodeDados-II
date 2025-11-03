using SistemaTransporteAereo.Domain.Entities;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Services.Interfaces
{
    public interface IClienteService
    {
        void CriarCliente(Cliente cliente);
        Cliente ObterClientePorId(int id);
        Cliente ObterClientePorCPF(string cpf);
        IEnumerable<Cliente> ObterTodosClientes();
        IEnumerable<Cliente> ObterClientesPreferenciais();
        void AtualizarCliente(Cliente cliente);
    }
}