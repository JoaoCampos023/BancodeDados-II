using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Banco_II.Models
{
    public class Subject
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        // Chave estrangeira para o curso
        public int CourseID { get; set; }

        // Propriedade de navegação
        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }
    }
}