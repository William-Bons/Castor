using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml.Serialization;

namespace Castor.database.tables
{
    [Serializable]
    public class DictPlannings
    {
        [Key] public int keyid { get; set; }
        public string description { get; set; } = string.Empty;
        public long? docdepid { get; set; }
        public bool isprivate { get; set; } = true;
        public int period { get; set; }

        
    }
}
