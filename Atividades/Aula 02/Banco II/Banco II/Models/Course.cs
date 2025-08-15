using System.ComponentModel.DataAnnotations;

namespace Banco_II.Models
{
    public class Course
    {
        [Key]
        public int ID { get; set; }
        public string? Name { get; set; }

        public List<StudentCourses>? StudentCourses { get; set; }
    }
}
