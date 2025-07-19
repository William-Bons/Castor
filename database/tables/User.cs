using System.ComponentModel;

namespace Castor.database.tables
{
    public class User : MetaTable
    {
        [Description("no")] public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        [Description("no")] public string? Password { get; set; }
        public int? Department { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expired { get; set; } = new DateTime(2031, 12, 31);
        public Int64 Rights { get; set; } = 0;
    }
}
