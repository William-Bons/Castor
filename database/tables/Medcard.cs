using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class Medcard
    {
        public int Id { get; set; }
        public Person? Person { get; set; }
        public User? User { get; set; }
        public int Department {  get; set; }
        public DateTime Enroll {  get; set; }
        public DateTime? Checkout { get; set; }
        public int? Involuntarily { get; set; } // недоброволка
        public int? Incapacitated { get; set; } // недееспособность
        public int? Disability { get; set; } // инвалидность
        public int? Bulletin { get; set; } // ЭЛН


    }
}
