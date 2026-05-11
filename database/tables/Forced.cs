using Castor.gui.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class Forced : ITableView
    {
        [Key] public long Id { get; set; }
        public string? Number { get; set; } // номер (идентификатор) дела/постановления
        public long RootId { get; set; } = 0; // ссылка на самое первое постановление
        public DateOnly Start {  get; set; }// начало принуд лечения (дата текущего пост суда)
        public DateOnly? End { get; set; } // дата закрытия этого постановления, заполняется после получения следующего пост суда 
        public long Patientid { get; set; } // id patient from medis
        public long Visitid { get; set; } // id visit from medis № и/бол
        public int? Type { get; set; } // 100-амб, 101-общ, 102 - спец, 103-стин, 104-снято, 199-в переходе, 
        public string? Courtname {  get; set; }// название суда
        public long? Movebookid { get; set; }// привязув к Id Movebook
        
        
        public virtual Movebook? Movebook { get; set; }
        public virtual int DaysTotal => End.HasValue ? (End.Value.ToDateTime(TimeOnly.MinValue)-Start.ToDateTime(TimeOnly.MinValue)).Days+1 : 0;
        public virtual int DaysToday => (DateTime.Today - Start.ToDateTime(TimeOnly.MinValue)).Days+1;
        public virtual int Months => (DateTime.Today.Month - Nextvk.Month) + 12 * (DateTime.Today.Year - Nextvk.Year);
        public virtual DateOnly Nextvk => Start.AddMonths(6); // рассчетная дата окончания действия постановления
        

        
    }
}
