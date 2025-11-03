using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaTransporteAereo.Domain.Entities;
using SistemaTransporteAereo.Domain.Interfaces;
using SistemaTransporteAereo.Infra.Data.Context;

namespace SistemaTransporteAereo.Infra.Data.Repositories
{
    public class VooRepository : Repository<Voo>, IVooRepository
    {
        public VooRepository(TransporteAereoContext context) : base(context) { }

        public Voo GetByCodigo(string codigoVoo)
        {
            return DbSet.FirstOrDefault(v => v.CodigoVoo == codigoVoo);
        }

        public IEnumerable<Voo> GetVoosDisponiveis()
        {
            return DbSet.Where(v => v.PoltronasDisponiveis > 0 && v.Status == "Agendado").ToList();
        }

        public IEnumerable<Voo> GetVoosPorData(DateTime data)
        {
            return DbSet.Where(v => DbFunctions.TruncateTime(v.DataHoraPartida) == data.Date).ToList();
        }

        public IEnumerable<Voo> GetVoosPorOrigemDestino(string origem, string destino, DateTime data)
        {
            return DbSet.Where(v => v.AeroportoOrigem.Codigo == origem &&
                                   v.AeroportoDestino.Codigo == destino &&
                                   DbFunctions.TruncateTime(v.DataHoraPartida) == data.Date &&
                                   v.PoltronasDisponiveis > 0 &&
                                   v.Status == "Agendado")
                       .ToList();
        }
    }
}