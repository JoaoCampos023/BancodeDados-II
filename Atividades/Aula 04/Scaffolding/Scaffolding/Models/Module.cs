using System;
using System.Collections.Generic;

namespace Scaffolding.Models;

public partial class Module
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? WorkloadHours { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual ICollection<CourseModule> CourseModules { get; set; } = new List<CourseModule>();

    public virtual ICollection<StudentModule> StudentModules { get; set; } = new List<StudentModule>();

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<Module> Prerequisites { get; set; } = new List<Module>();
}
