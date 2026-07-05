using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    /// <summary>
    /// Класс описывает даты и необходимость проведенеия цикличных ВК по НГ и по ЭЛН
    /// </summary>
    public class Commity
    {
        [Key]
        public long Id { get; set; }            // Primary Key!
        public long MovebookId { get; set; }    // Foreign key to Movebook Id
        public long Visitid { get; set; }       // visitid from medis 
        public long Patientid { get; set; }     // patient id from medis
        public int? Type { get; set; }          // ВИД ВК:  100 - ЭЛН, 101 - НГ 1 мес, 102 - НГ 6 мес, 199 - общий ВК (период устанавливается польз.) 
        public DateOnly? Start { get; set; }    // дата начала отсчета, обычно это дата поступления в стационар, или дата оформления НГ 
        public DateOnly? Closed { get; set; }   // дата закрытия, если заполнена - цикл закончен

        public virtual IEnumerable<Runs>? Runs { get; set; }
        public virtual Movebook? Movebook { get; set; }
        public virtual TimeSpan Interval
        {
            get
            {
                switch (Type)
                {
                    case 100: return TimeSpan.FromDays(15);
                    case 101: return TimeSpan.FromDays(30);
                    case 102: return TimeSpan.FromDays(183);
                    default:  return TimeSpan.Zero;
                }
            }
        }

        public DateOnly CalculateNextIteration(DateOnly fromDate)
        {
            DateOnly d = fromDate.AddDays(Interval.Days);
            if (d.DayOfWeek == DayOfWeek.Sunday) d = d.AddDays(-2);
            if (d.DayOfWeek == DayOfWeek.Saturday) d = d.AddDays(-1);
            return d;
        }
            
    }
}
