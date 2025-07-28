using System.ComponentModel.DataAnnotations;

namespace Castor.database.tables
{
    [Serializable]
    public class dictionary
    {
        public enum DicType { PLANNING };
        [Key] public int keyid { get; set; }
        public int dictype { get; set; } = (int)DicType.PLANNING;
        public string description { get; set; } = string.Empty;
        public long? docdepid { get; set; }
        public bool isprivate { get; set; } = true;
        public int period { get; set; }

        public virtual ICollection<planning>? Plannings { get; set; }
    }
}
