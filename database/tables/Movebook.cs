using System;
using System.Collections.Generic;

namespace Castor.database.tables;

public partial class Movebook
{
    public int Id { get; set; }
    public int Card_Id { get; set; }
    public int? Patientid { get; set; }
    public string Fio { get; set; } = null!;
    public DateOnly? Birthdate { get; set; }
    public DateOnly? Datein { get; set; }
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
    public string? Date_Lastout { get; set; }
    public int? Closed { get; set; }
    public int? Days { get; set; }
    public virtual int? Agein { get; set; }
    public virtual int? Ageout { get; set; }
    public virtual int? Ai { get; set; }
    public virtual int? Bi { get; set; }
    public virtual int? Ci { get; set; }
    public virtual int? Di { get; set; }
    public virtual int? Ei { get; set; }
    public virtual string? Contr_In { get; set; }
    public virtual int? Ao { get; set; }
    public virtual int? Bo { get; set; }
    public virtual int? Co { get; set; }
    public virtual int? Do { get; set; }
    public virtual int? Eo { get; set; }
    public virtual string? Contr_Out { get; set; }
}
