using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int Department {  get; set; }
        public DateTime Created {  get; set; }
        public DateTime Expired { get; set; }
        public Int64 Rights { get; set; } 
    }
}
