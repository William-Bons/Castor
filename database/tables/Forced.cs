using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class Forced
    {
        public long Id { get; set; }
        public DateOnly Start {  get; set; }
        public DateOnly? End { get; set; }
        public DateOnly? Lastday { get; set; }
        public virtual int DaysTotal => End.HasValue ? (End.Value.ToDateTime(TimeOnly.MinValue)-Start.ToDateTime(TimeOnly.MinValue)).Days+1 : 0;
        public virtual int DaysToday => (DateTime.Today - Start.ToDateTime(TimeOnly.MinValue)).Days+1;
        public virtual int Months => Lastday.HasValue ? (DateTime.Today.Month - Lastday.Value.Month) + 12 * (DateTime.Today.Year - Lastday.Value.Year) : 0;

        
    }
}
