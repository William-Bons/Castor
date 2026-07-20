using Castor.gui.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{
    public class Forced : ITableView
    {
        [Key] public long Id { get; set; }
        public string? Number { get; set; } // номер (идентификатор) дела/постановления
        public long? RootId { get; set; } // ссылка на самое первое постановление
        public DateOnly Start { get; set; }// начало принуд лечения (дата текущего пост суда)
        public DateOnly? End { get; set; } // дата закрытия этого постановления, заполняется после получения следующего пост суда 
        public long Patientid { get; set; } // id patient from medis
        public long Visitid { get; set; } // id visit from medis № и/бол
        public int? Type { get; set; } // 100-амб, 101-общ, 102 - спец, 103-стин, 104-снято, 199-в переходе, 
        public string? Section { get; set; } // статья(и) уголовного кодекса с комментарием
        public string? Courtname { get; set; }// название суда
        public long? Movebookid { get; set; }// привязув к Id PatientRecord
        public int? MonthFlag { get; set; }// номер месяца для рассчетного постановления обязательно уст у Root


        public virtual ICollection<Forced>? AllForces { get; set; }

        [ForeignKey(nameof(RootId))]
        public virtual Forced? RootForced { get; set; }
        public virtual Movebook? Movebook { get; set; }
        public virtual int[]? Month => [(MonthFlag ?? 1), (((MonthFlag ?? 1) - 1 + 6) % 12) + 1];


        public virtual string DisplayText => ToString();
        public override string ToString()
        {
            // Формируем читаемую строку для узла дерева
            var parts = new System.Text.StringBuilder();

            if (MonthFlag != null)
                parts.Append("> ");

            if (!string.IsNullOrEmpty(Number))
                parts.Append($"№ {Number}");

            if (parts.Length > 0)
                parts.Append(" | ");

            parts.Append($"Тип: {Type}");

            if (Start != default)
                parts.Append($" | {Start.ToString("dd.MM.yyyy")}");

            return parts.ToString();
        }

    }
}
