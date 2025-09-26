using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banco_II.Models
{
    public class Course
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "O nome do curso é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres")]
        public string? Name { get; set; }

        public List<StudentCourses>? StudentCourses { get; set; }

        // CORREÇÃO: Remover InverseProperty e inicialização forçada
        public virtual ICollection<Subject>? Subjects { get; set; }
    }
}