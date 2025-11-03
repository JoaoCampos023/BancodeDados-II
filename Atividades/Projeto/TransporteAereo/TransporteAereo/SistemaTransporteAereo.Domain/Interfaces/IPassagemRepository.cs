using SistemaTransporteAereo.Domain.Entities;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Domain.Interfaces
{
    public interface IPassagemRepository : IRepository<Passagem>
    {
        Passagem GetByCodigoReserva(string codigoReserva);
        IEnumerable<Passagem> GetPassagensPorCliente(int clienteId);
        IEnumerable<Passagem> GetPassagensPorVoo(int vooId);
        IEnumerable<string> GetPoltronasOcupadas(int vooId);
    }
}