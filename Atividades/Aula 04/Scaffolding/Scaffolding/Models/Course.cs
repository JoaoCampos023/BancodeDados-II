using System;
using System.Collections.Generic;

namespace Scaffolding.Models;

public partial class Course
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime CreationDate { get; set; }

    public int NumberOfSemesters { get; set; }

    public virtual ICollection<CourseModule> CourseModules { get; set; } = new List<CourseModule>();

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();

    public virtual ICollection<StudentModule> StudentModules { get; set; } = new List<StudentModule>();
}
