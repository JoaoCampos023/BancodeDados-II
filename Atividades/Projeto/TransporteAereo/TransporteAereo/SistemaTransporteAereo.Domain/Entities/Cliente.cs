using System;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Domain.Entities
{
    public class Cliente
    {
        public int ClienteId { get; private set; }
        public string Nome { get; private set; }
        public string CPF { get; private set; }
        public string Email { get; private set; }
        public string Telefone { get; private set; }
        public bool ClientePreferencial { get; private set; }
        public DateTime DataCadastro { get; private set; }

        // Navigation properties
        public virtual ICollection<Passagem> Passagens { get; private set; }

        public Cliente() { }

        public Cliente(string nome, string cpf, string email, string telefone, bool clientePreferencial)
        {
            Nome = nome;
            CPF = cpf;
            Email = email;
            Telefone = telefone;
            ClientePreferencial = clientePreferencial;
            DataCadastro = DateTime.Now;
            Passagens = new HashSet<Passagem>();
        }

        public void AtualizarDados(string nome, string email, string telefone, bool clientePreferencial)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            ClientePreferencial = clientePreferencial;
        }
    }
}