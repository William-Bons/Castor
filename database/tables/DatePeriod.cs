namespace Castor.database.tables;

public class DatePeriod
{
    public bool Set {  get; set; }
    public DateTime Start { get; set; } 
    public DateTime End { get; set; }

    public DatePeriod(bool calculate=true)
    {
        if(calculate)
        {
            Start = CalcStart();
            End = CalcEnd();
            Set = true;
        }
    }

    public DateTime CalcStart()
    {
        return DateTime.Parse($"{21}.{(DateTime.Today.Month > 1 ? DateTime.Today.Month - 1 : 12)}.{DateTime.Today.Year}");
    }

    public DateTime CalcEnd()
    {
        return DateTime.Parse($"{20}.{DateTime.Today.Month}.{DateTime.Today.Year}");
    }
}

