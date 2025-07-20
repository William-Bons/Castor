using System.ComponentModel;

namespace Castor.database.tables
{
    public class User : MetaTable
    {
        private static User? _CurrentUser;

        [Description("no")] public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        [Description("no")] public string? Password { get; set; }
        public int? Department { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ExpiredDate { get; set; } = new DateTime(2031, 12, 31);
        public Int64 Rights { get; set; } = 0;
        [Description("no")] public ICollection<Person>? Persons { get; }
        [Description("no")] public ICollection<Medcard>? Medcards { get; }

        public static User CurrentUser()
        {
            return _CurrentUser;
        }
        public static void SetCurrent(User u)
        {
            _CurrentUser = u;

        }
    }
}
