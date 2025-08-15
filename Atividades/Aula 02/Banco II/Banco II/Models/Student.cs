using System.ComponentModel.DataAnnotations;

namespace Banco_II.Models
{
    public class Student
    {
        [Key]
        public int ID { get; set; }
        public string? LastName { get; set; }
        public string? FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public List<StudentCourses>? StudentCourses { get; set; }

    }
}
