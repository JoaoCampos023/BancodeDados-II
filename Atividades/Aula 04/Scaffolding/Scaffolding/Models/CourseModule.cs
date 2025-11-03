using System;
using System.Collections.Generic;

namespace Scaffolding.Models;

public partial class CourseModule
{
    public int CourseId { get; set; }

    public int ModuleId { get; set; }

    public int Semester { get; set; }

    public int? DayOfWeek { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;
}
