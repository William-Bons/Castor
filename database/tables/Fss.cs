using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class Fss
    {
        public long Id { get; set; }
        public DateOnly Start {  get; set; }
        public DateOnly? End { get; set; }
        public DateOnly? Nextvk { get; set; }
        public int ElnDay => (DateTime.Today - Start.ToDateTime(TimeOnly.MinValue)).Days + 1;


        public DateOnly CalcNextVk(DateOnly current)
        {
            DateTime nd = current.ToDateTime(TimeOnly.MinValue) + TimeSpan.FromDays(15);
            nd = nd - TimeSpan.FromDays(
                nd.DayOfWeek == DayOfWeek.Sunday ? 2 :
                nd.DayOfWeek == DayOfWeek.Saturday ? 1 :
                0
                );
            return DateOnly.FromDateTime(nd);
        }
    }
}
