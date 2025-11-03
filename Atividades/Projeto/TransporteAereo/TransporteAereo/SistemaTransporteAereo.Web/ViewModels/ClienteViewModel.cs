using System.ComponentModel.DataAnnotations;

namespace SistemaTransporteAereo.Web.ViewModels
{
    public class ClienteViewModel
    {
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(11, ErrorMessage = "CPF deve ter 11 caracteres")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        public string Telefone { get; set; }
        public bool ClientePreferencial { get; set; }
    }
}