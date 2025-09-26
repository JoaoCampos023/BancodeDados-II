using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banco_II.Models
{
    public class Subject
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "O nome da matéria é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        // CORREÇÃO: FK explícita sem inicialização forçada
        [ForeignKey("Course")]
        public int CourseID { get; set; }

        // CORREÇÃO: Navegação virtual SEM inicialização
        public virtual Course? Course { get; set; }
    }
}