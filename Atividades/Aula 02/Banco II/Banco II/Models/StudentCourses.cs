using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Banco_II.Models
{
    [PrimaryKey(nameof(StudentID), nameof(CourseID))]
    public class StudentCourses
    {
        public int StudentID { get; set; }


        // Proprety Navigatios
        [ForeignKey(nameof(StudentID))]
        public Student? Student { get; set; }

        public int CourseID { get; set; }
        
        [ForeignKey(nameof(CourseID))]
        public Course? Course { get; set; }
    }
}
