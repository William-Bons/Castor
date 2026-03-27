using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database.tables
{

    public struct Quartal
    {
        public DateTime start;
        public DateTime end;
    }
    public class Bandbook
    {
        public long Id { get; set; }
        public long Movebookid { get; set; }
        public string? point01 { get; set; }
        public string? point02 { get; set; }
        public string? point03 { get; set; }
        public string? point04 { get; set; }
        public string? point05 { get; set; }
        public string? point06 { get; set; }
        public string? point07 { get; set; }
        public string? point08 { get; set; }
        public string? point09 { get; set; }
        public string? point10 { get; set; }
        public string? point11 { get; set; }
        public string? point12 { get; set; }
        public string? point13 { get; set; }
        public string? point14 { get; set; }
        public string? point15 { get; set; }
        public string? point16 { get; set; }
        public string? point17 { get; set; }
        public string? point18 { get; set; }
        public string? point19 { get; set; }
        public string? point20 { get; set; }

        public virtual Movebook? Movebook {  get; set; }
    }
}
