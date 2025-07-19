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
        public DateTime Created {  get; set; } = DateTime.Now;
        public DateTime? Updated { get; set; }
        public int Planntype { get; set; }
        public DateTime Show {  get; set; }
        public DateTime? Next { get; set; }
        public int? NextId { get; set; }
        public string? Description { get; set; }
        public bool Executed { get; set; } = false;

    }
}
