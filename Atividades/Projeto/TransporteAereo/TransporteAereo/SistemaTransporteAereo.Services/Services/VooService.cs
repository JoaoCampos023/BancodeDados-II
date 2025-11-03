using SistemaTransporteAereo.Domain.Entities;
using SistemaTransporteAereo.Domain.Interfaces;
using SistemaTransporteAereo.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Services.Services
{
    public class VooService : IVooService
    {
        private readonly IVooRepository _vooRepository;
        private readonly IAviaoRepository _aviaoRepository;

        public VooService(IVooRepository vooRepository, IAviaoRepository aviaoRepository)
        {
            _vooRepository = vooRepository;
            _aviaoRepository = aviaoRepository;
        }

        public void AgendarVoo(Voo voo)
        {
            var aviao = _aviaoRepository.GetById(voo.AviaoId);
            if (aviao == null)
                throw new InvalidOperationException("Avião não encontrado");

            voo.AtualizarPoltronasDisponiveis(aviao.QuantidadePoltronas);
            _vooRepository.Add(voo);
        }

        public Voo ObterVooPorId(int id)
        {
            return _vooRepository.GetById(id);
        }

        public IEnumerable<Voo> ObterTodosVoos()
        {
            return _vooRepository.GetAll();
        }

        public IEnumerable<Voo> ObterVoosDisponiveis()
        {
            return _vooRepository.GetVoosDisponiveis();
        }

        public IEnumerable<Voo> BuscarVoos(string origem, string destino, DateTime data)
        {
            return _vooRepository.GetVoosPorOrigemDestino(origem, destino, data);
        }

        public void AtualizarStatusVoo(int vooId, string status)
        {
            var voo = _vooRepository.GetById(vooId);
            if (voo != null)
            {
                voo.AtualizarStatus(status);
                _vooRepository.Update(voo);
            }
        }
    }
}