using SistemaTransporteAereo.Domain.Entities;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Domain.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Cliente GetByCPF(string cpf);
        Cliente GetByEmail(string email);
        IEnumerable<Cliente> GetClientesPreferenciais();
    }
}