using System;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Castor.database.tables;

public struct DatePeriod
{
    public bool Set;
    public DateTime Start;
    public DateTime End;
}
public class Movebook
{
    public long Id { get; set; }
    public long Card_Id { get; set; }
    public long? Patientid { get; set; }
    public long? Visitid { get; set; }
    public string Fio { get; set; } = null!;
    public DateOnly? Birthdate { get; set; }
    public DateOnly? Datein { get; set; }
    public DateOnly? Dateout { get; set; }
    public int? Ordered { get; set; }
    public string? Dsin { get; set; } = null!;
    public string? Dsout { get; set; }
    public int? Outto { get; set; }
    public bool City { get; set; }
    public bool First { get; set; }
    public bool Second { get; set; }
    public bool Early { get; set; }
    public bool Unvoluntary { get; set; }
    public DateOnly? Date_Lastout { get; set; }
    public bool Closed { get; set; }
    public bool Deceased { get; set; }
    public virtual int? Agein => Datein.HasValue ? Datein.Value.Year - Birthdate.Value.Year - (Datein.Value.Month < Birthdate.Value.Month && Datein.Value.Day < Birthdate.Value.Day ? 1 : 0) : null;//todo WRONG!
    public virtual int? Ageout => Dateout.HasValue ? Dateout.Value.Year - Birthdate.Value.Year - (Dateout.Value.Month < Birthdate.Value.Month && Dateout.Value.Day < Birthdate.Value.Day ? 1 : 0) : null;
    public int? Days => (Datein.HasValue && Dateout.HasValue) ? (Dateout.Value.ToDateTime(TimeOnly.MinValue) - Datein.Value.ToDateTime(TimeOnly.MinValue)).Days : null;
    public virtual int Ai => (Dsin == "21" || Dsin == "22" || Dsin == "23" || Dsin == "25" || Dsin == "30" || Dsin == "31" || Dsin == "32" || Dsin == "33" || Dsin == "00" || Dsin == "01") ? 1 : 0;
    public virtual int? Bi => (Dsin == "20") ? 1 : 0;
    public virtual int? Ci  => Dsin == "70" || Dsin == "71" || Dsin == "72" ? 1 : 0;
    public virtual int? Di => Dsin == "02" || Dsin == "03" || Dsin == "04" || Dsin == "05" || Dsin == "06" || Dsin == "07" || Dsin == "50" || Dsin == "60" || Dsin == "61" || Dsin == "62" || Dsin == "90" || Dsin == "91" || Dsin == "40" || Dsin == "41" || Dsin == "42" || Dsin == "43" || Dsin == "45" || Dsin == "48" ? 1 : 0;
    public virtual int? Ei => Dsin == "10" || Dsin == "15" || Dsin == "19" ? 1 : 0;
    public virtual int? Fi => Dsin == "10" ? 1 : 0;
    public virtual int? Ao => Dsout == "21" || Dsout == "22" || Dsout == "23" || Dsout == "25" || Dsout == "30" || Dsout == "31" || Dsout == "32" || Dsout == "33" || Dsout == "00" || Dsout == "01" ? 1 : 0;
    public virtual int? Bo => Dsout == "20" ? 1 : 0;
    public virtual int? Co => Dsout == "70" || Dsout == "71" || Dsout == "72" ? 1 : 0;
    public virtual int? Do => Dsout == "02" || Dsout == "03" || Dsout == "04" || Dsout == "05" || Dsout == "06" || Dsout == "07" || Dsout == "50" || Dsout == "60" 
	|| Dsout == "61" || Dsout == "62" || Dsout == "90" || Dsout == "91" || Dsout == "40" || Dsout == "41" || Dsout == "42" || Dsout == "43" || Dsout == "45" || Dsout == "48" ? 1 : 0;
    public virtual int? Eo => Dsout == "10" || Dsout == "15" || Dsout == "19" ? 1 : 0;
    public virtual int? Fo => Dsout == "10" ? 1 : 0;

    public virtual bool Ctri => Ai + Bi + Ci + Di + Ei == 1;

    public virtual bool Ctro => Ao + Bo + Co + Do + Eo == 1;
}

