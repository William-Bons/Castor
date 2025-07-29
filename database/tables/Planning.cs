using System.ComponentModel.DataAnnotations;

namespace Castor.database.tables
{
    public class planning
    {
        [Key] public int keyid { get; set; }
        public long patientid { get; set; }     // patient
        public long docdepid { get; set; }      // doctor
        public long visitid { get; set; }       // visit
        public long depid { get; set; }         // department
        public DateTime created_date { get; set; } = DateTime.Now;
        public int dictionaryid { get; set; }   // planning type id
        public DateTime? start_date { get; set; } // date started
        public DateTime? next_date { get; set; } 
        public int cycles { get; set; } = 0;    // num of iteration
        public string? description { get; set; } // 
        public bool executed { get; set; } = false;  // if finished

        public virtual dictionary? Dictionary { get; set; }
        public virtual int DaysInDep => start_date != null ? (DateTime.Now - start_date).Value.Days : 0;
        public virtual int DaysToNext => next_date != null && created_date != null ? (next_date - created_date).Value.Days+1 : 0;

    }
}
