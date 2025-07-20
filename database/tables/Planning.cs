using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class Planning
    {
        public int Id { get; set; }
        public DateTime CreatedDate {  get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public int Planntype { get; set; }
        public DateTime ShowDate {  get; set; }
        public DateTime? NextDate { get; set; }
        public int? NextId { get; set; }
        public string? Description { get; set; }
        public bool Executed { get; set; } = false;

    }
}
