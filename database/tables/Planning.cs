using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Castor.database.tables;

public partial class Planning
{
    [Key] public int Keyid { get; set; }

    public int Patientid { get; set; }

    public int Docdepid { get; set; }

    public int Visitid { get; set; }

    public int Depid { get; set; }

    public string CreatedDate { get; set; } = null!;

    public int Plantype { get; set; }

    public string StartDate { get; set; } = null!;

    public string? NextDate { get; set; }

    public int Cycles { get; set; }

    public string? Description { get; set; }

    public int Executed { get; set; }
}
