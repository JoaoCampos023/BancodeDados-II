using SistemaTransporteAereo.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Services.Interfaces
{
    public interface IVooService
    {
        void AgendarVoo(Voo voo);
        Voo ObterVooPorId(int id);
        IEnumerable<Voo> ObterTodosVoos();
        IEnumerable<Voo> ObterVoosDisponiveis();
        IEnumerable<Voo> BuscarVoos(string origem, string destino, DateTime data);
        void AtualizarStatusVoo(int vooId, string status);
    }
}