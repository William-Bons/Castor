using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Castor.database.tables;
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
    public DateOnly? Date_Lastout { get; set; }
    public bool Closed { get; set; }
    public bool Deceased { get; set; }
    public long? Fssid { get; set; }
    public long? Unvoluntaryid { get; set; }
    public virtual int? Agein => CalculateAge(Datein);
    public virtual int? Ageout => CalculateAge(Dateout);
    public int? Days => (Datein.HasValue && Dateout.HasValue) ? (Dateout.Value.ToDateTime(TimeOnly.MinValue) - Datein.Value.ToDateTime(TimeOnly.MinValue)).Days : null;
    public int? DaysToday => Datein.HasValue && !Dateout.HasValue ? (DateTime.Today - Datein.Value.ToDateTime(TimeOnly.MinValue)).Days : null;
    public bool? InControl => string.IsNullOrWhiteSpace(Dsin) ? null : calc0(Dsin).Take(5).Count(x => x) == 1;
    public bool? OutControl => string.IsNullOrWhiteSpace(Dsout) ? null : calc0(Dsout).Take(5).Count(x => x) == 1;
    public virtual Fss? FssControl {  get; set; }
    public virtual ICollection<Forced>? Forceds { get; set; }
    public virtual Unvoluntary? UnvoluntaryControl { get; set; }
    public virtual ICollection<Commity> Commities { get; set; }


    /// <summary>
    /// Create array checking diagnisis in input line; 
    /// </summary>
    /// <param name="input"></param>
    /// <returns>checked array if true only one anotherway returns null</returns>
    public bool[] calc0(string input)
    {
        try
        {
            bool[] result =
            [
         /*A*/   Regex.IsMatch(input, @"^F(21|22|23|25|30|31|32|33|00|01)"),                                //Психозы + состояние слабоумия старческого  возраста
         /*B*/   Regex.IsMatch(input, @"^F(20)"),                                                           //Шизофрения
         /*C*/   Regex.IsMatch(input, @"^F(70|71|72|73)"),                                                  //Умственная отсталость
         /*D*/   Regex.IsMatch(input, @"^F(02|03|04|05|06|07|50|60|61|62|90|91|40|41|42|43|45|48|65|88)"),  //Непсихотические расстройства
         /*E*/   Regex.IsMatch(input, @"^F1\d"),                                                            //В  связи с употреблением психоактивных веществ
         /*F*/   Regex.IsMatch(input, @"^F10"),                                                             //•	из них:   -  хронический Алкоголизм (F 10.1;  F 10.2; F 10.3 )
         /*G*/   Regex.IsMatch(input, @"^F1[1-9]"),                                                         //наркомании, токсикомании
            ];
            return result;
        }
        catch
        {
            return new bool[7];
        }
    }

    private int? CalculateAge(DateOnly? today)
    {
        if (today == null || Birthdate==null) return null;
        var age = today.Value.Year - Birthdate.Value.Year;
        // Go back to the year in which the person was born in the current date's month and day
        if (Birthdate.Value > today.Value.AddYears(-age))
        {
            age--;
        }
        return age;
    }

}

