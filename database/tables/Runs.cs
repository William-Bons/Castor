using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{

    /// <summary>
    /// класс описывает реальные даты проведения ВК для записей в классе Commity
    /// </summary>
    public class Runs
    {
        [Key]
        public long Id { get; set; }
        public long CommityId { get; set; }
        public int? Type { get; set; }          // тот же тип что и в Commity
        public DateOnly? RealDate { get; set; } // дата реального заседания
        public string? Number { get; set; }
        public string? Text { get; set; }

        public virtual Commity? Commity { get; set; }

    }
}
