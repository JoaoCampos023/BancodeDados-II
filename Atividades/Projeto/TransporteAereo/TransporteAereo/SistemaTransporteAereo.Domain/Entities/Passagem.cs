using System;

namespace SistemaTransporteAereo.Domain.Entities
{
    public class Passagem
    {
        public int PassagemId { get; private set; }
        public string CodigoReserva { get; private set; }
        public int ClienteId { get; private set; }
        public int VooId { get; private set; }
        public DateTime DataCompra { get; private set; }
        public string Status { get; private set; }
        public string NumeroPoltrona { get; private set; }
        public bool DespacheMala { get; private set; }
        public decimal PesoMala { get; private set; }
        public string LocalEmbarque { get; private set; }
        public DateTime? DataHoraEmbarque { get; private set; }
        public bool EmbarquePrioritario { get; private set; }

        // Navigation properties
        public virtual Cliente Cliente { get; private set; }
        public virtual Voo Voo { get; private set; }

        public Passagem() { }

        public Passagem(int clienteId, int vooId, bool despacheMala, decimal pesoMala,
                       string localEmbarque, bool embarquePrioritario)
        {
            ClienteId = clienteId;
            VooId = vooId;
            DataCompra = DateTime.Now;
            Status = "Reservada";
            CodigoReserva = GerarCodigoReserva();
            DespacheMala = despacheMala;
            PesoMala = pesoMala;
            LocalEmbarque = localEmbarque;
            EmbarquePrioritario = embarquePrioritario;
        }

        public void AtribuirPoltrona(string numeroPoltrona)
        {
            NumeroPoltrona = numeroPoltrona;
        }

        public void RealizarEmbarque()
        {
            DataHoraEmbarque = DateTime.Now;
            Status = "Confirmada";
        }

        public void Cancelar()
        {
            Status = "Cancelada";
        }

        private string GerarCodigoReserva()
        {
            return $"RES{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }
    }
}