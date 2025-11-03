using System;
using System.Collections.Generic;

namespace Scaffolding.Models;

public partial class StudentModule
{
    public int StudentId { get; set; }

    public int ModuleId { get; set; }

    public int CourseId { get; set; }

    public double? Grade { get; set; }

    public double? Frequency { get; set; }

    public int? DayOfWeek { get; set; }

    public DateTime SignDate { get; set; }

    public DateTime? CancelDate { get; set; }

    public int Status { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
