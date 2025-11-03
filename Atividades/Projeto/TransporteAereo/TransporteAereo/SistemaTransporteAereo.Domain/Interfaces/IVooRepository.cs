using SistemaTransporteAereo.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Domain.Interfaces
{
    public interface IVooRepository : IRepository<Voo>
    {
        Voo GetByCodigo(string codigoVoo);
        IEnumerable<Voo> GetVoosDisponiveis();
        IEnumerable<Voo> GetVoosPorData(DateTime data);
        IEnumerable<Voo> GetVoosPorOrigemDestino(string origem, string destino, DateTime data);
    }
}