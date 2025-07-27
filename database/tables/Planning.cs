using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class Planning
    {
        [Key] public int keyid { get; set; }
        public long patientid { get; set; }
        public long docdepid { get; set; }
        public long visitid { get; set; }
        public long depid { get; set; }
        public DateTime created_date {  get; set; } = DateTime.Now;
        public int plantype { get; set; }
        public DateTime start_date {  get; set; }
        public DateTime? next_date { get; set; }
        public int cycles { get; set; } = 0;
        public string? description { get; set; }
        public bool executed { get; set; } = false;

    }
}
