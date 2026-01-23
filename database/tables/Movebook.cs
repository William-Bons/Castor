using System;
using System.Collections.Generic;

namespace Castor.database.tables;

public partial class Movebook
{
    public int Id { get; set; }

    public int Card_Id { get; set; }

    public int? Patientid { get; set; }

    public string Fio { get; set; } = null!;

    public DateOnly Birthdate { get; set; }

    public DateOnly Datein { get; set; }

    public DateOnly? Dateout { get; set; }

    public int? Ordered { get; set; }

    public string Dsin { get; set; } = null!;

    public string? Dsout { get; set; }

    public int? Outto { get; set; }

    public int? City { get; set; }

    public int? First { get; set; }

    public decimal? Second { get; set; }

    public int? Early { get; set; }

    public int? Unvoluntary { get; set; }

    public DateOnly? Date_Lastout { get; set; }

    public int? Agein { get; set; }

    public int? Ageout { get; set; }

    public int? Ai { get; set; }

    public int? Bi { get; set; }

    public int? Ci { get; set; }

    public int? Di { get; set; }

    public int? Ei { get; set; }

    public string? Contr_In { get; set; }

    public int? Ao { get; set; }

    public int? Bo { get; set; }

    public int? Co { get; set; }

    public int? Do { get; set; }

    public int? Eo { get; set; }

    public string? Contr_Out { get; set; }
}
