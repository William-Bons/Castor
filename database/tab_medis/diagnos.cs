using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Castor.database.tab_medis
{
    public class diagnos
    {
        [Key] public long keyid {get; set; }
        public long? createby {get; set; }
        public DateTime? createdate {get; set; }
        public long? updateby {get; set; }
        public DateTime? updatedate {get; set; }
        public long? rootid {get; set; }
        public string? lev {get; set; }
        public string? code {get; set; }
        public string? text {get; set; }
        public string? note {get; set; }
        public string? code_group1 {get; set; }
        public string? code_group2 {get; set; }
        public long? code_group1_id {get; set; }
        public long? code_group2_id {get; set; }
        public string? block {get; set; }
        public string? block_name {get; set; }
        public int? oms_status {get; set; }
        public long? depid {get; set; }
        public long? docdepid {get; set; }
        public long? diag_ref_id {get; set; }
        public long? mes_id {get; set; }
        public long? lu_id {get; set; }
        public int? age_from {get; set; }
        public int? age_to {get; set; }
        public int? chronic_status {get; set; }
        public int? sex {get; set; }
        public int? days {get; set; }
        public long? report_group_id {get; set; }
        public int? status {get; set; }
        public int? not_main_status {get; set; }
        public DateTime? bgndat {get; set; }
        public DateTime? enddat {get; set; }
    }
}
