using System;
using System.Collections.Generic;

namespace SistemaTransporteAereo.Domain.Entities
{
    public class Voo
    {
        public int VooId { get; private set; }
        public string CodigoVoo { get; private set; }
        public int AeroportoOrigemId { get; private set; }
        public int AeroportoDestinoId { get; private set; }
        public int AviaoId { get; private set; }
        public DateTime DataHoraPartida { get; private set; }
        public DateTime DataHoraChegada { get; private set; }
        public int DuracaoMinutos { get; private set; }
        public string Status { get; private set; }
        public int PoltronasDisponiveis { get; private set; }

        // Navigation properties
        public virtual Aeroporto AeroportoOrigem { get; private set; }
        public virtual Aeroporto AeroportoDestino { get; private set; }
        public virtual Aviao Aviao { get; private set; }
        public virtual ICollection<Passagem> Passagens { get; private set; }

        public Voo() { }

        public Voo(string codigoVoo, int aeroportoOrigemId, int aeroportoDestinoId, int aviaoId,
                  DateTime dataHoraPartida, DateTime dataHoraChegada)
        {
            CodigoVoo = codigoVoo;
            AeroportoOrigemId = aeroportoOrigemId;
            AeroportoDestinoId = aeroportoDestinoId;
            AviaoId = aviaoId;
            DataHoraPartida = dataHoraPartida;
            DataHoraChegada = dataHoraChegada;
            DuracaoMinutos = (int)(dataHoraChegada - dataHoraPartida).TotalMinutes;
            Status = "Agendado";
            PoltronasDisponiveis = 0; // Será definido pelo serviço
            Passagens = new HashSet<Passagem>();
        }

        public void AtualizarStatus(string status)
        {
            Status = status;
        }

        public void AtualizarPoltronasDisponiveis(int poltronas)
        {
            PoltronasDisponiveis = poltronas;
        }
    }
}