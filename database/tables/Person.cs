using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class Person : MetaTable
    {
        public enum Genders { NotSet, Male, Female};

        [Description("no")] public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SurName { get; set; }
        public DateTime? Birthday { get; set; }
        public byte? Gender { get; set; } = (byte)Genders.NotSet; // 0-not set, 1-male, 2-female
        public string? snils { get; set; }
        public string? Email { get; set; }
        [Description("no")] public string? FullName { get; set; }
        [Description("no")] public string? ShortName { get; set; }
        public string? Description { get; set; }
        [Description("no")] public int? Age { get; set; } = 0;
        public string? Address { get; set; }
        [Description("no")] public string? Fias { get; set; }
        public string? Phone { get; set; }
        public User? UserId { get; set; }

    }
}
