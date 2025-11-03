using SistemaTransporteAereo.Domain.Entities;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Services.Interfaces
{
    public interface IPassagemService
    {
        Passagem ComprarPassagem(int clienteId, int vooId, bool despacheMala, decimal pesoMala);
        Passagem ObterPassagemPorId(int id);
        Passagem ObterPassagemPorCodigoReserva(string codigoReserva);
        IEnumerable<Passagem> ObterPassagensPorCliente(int clienteId);
        void RealizarEmbarque(int passagemId);
        void CancelarPassagem(int passagemId);
    }
}