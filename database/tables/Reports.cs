using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class Reports
    {
        public long Id { get; set; }
        public int ReportCode { get; set; }
        public string? ReportName { get; set; }
        public DateTime? Created { get; set; }
        public DateOnly? DateStart { get; set; }
        public DateOnly? DateEnd { get; set; }
        public bool Printed { get; set; }
        public byte[]? ReportData { get; set; } 


    }
}
