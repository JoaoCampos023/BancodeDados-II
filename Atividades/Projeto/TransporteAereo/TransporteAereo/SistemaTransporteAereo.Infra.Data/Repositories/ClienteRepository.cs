using System.Collections.Generic;
using System.Linq;
using SistemaTransporteAereo.Domain.Entities;
using SistemaTransporteAereo.Domain.Interfaces;
using SistemaTransporteAereo.Infra.Data.Context;

namespace SistemaTransporteAereo.Infra.Data.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(TransporteAereoContext context) : base(context) { }

        public Cliente GetByCPF(string cpf)
        {
            return DbSet.FirstOrDefault(c => c.CPF == cpf);
        }

        public Cliente GetByEmail(string email)
        {
            return DbSet.FirstOrDefault(c => c.Email == email);
        }

        public IEnumerable<Cliente> GetClientesPreferenciais()
        {
            return DbSet.Where(c => c.ClientePreferencial).ToList();
        }
    }
}