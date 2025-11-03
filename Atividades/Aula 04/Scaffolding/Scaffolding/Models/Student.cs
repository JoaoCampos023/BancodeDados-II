using System;
using System.Collections.Generic;

namespace Scaffolding.Models;

public partial class Student
{
    public int Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstMidName { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    public virtual ICollection<StudentModule> StudentModules { get; set; } = new List<StudentModule>();
}
