using Castor.database.tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Castor.gui.movebook
{

    public struct Header
    {
        public string Name { get; set; }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
        public DateTime Created { get; set; }
    }

    public struct DataBlock
    {
        public string BlockName { get; set; }
        public int ValueA { get; set; }
        public int ValueB { get; set; }
        public int ValueC { get; set; }
        public int ValueD { get; set; }
        public int ValueE { get; set; }

    }

    [Obsolete]
    public class MonthOutput 
    {
        private MonthOutput() { }
        public MonthOutput(ICollection<Movebook> movebooks) 
        {
            Header h = new Header();
            h.Name = $"Отчет за период ";
            h.Created = DateTime.Now;
            Header = h ;

            DataAllIn = calculate(movebooks); // ALL ENTERED
            DataFirstIn = calculate(movebooks.Where(x => x.First==1).ToList());   // ENTERED FIRST
            DataLastIn = calculate(movebooks.Where(x => x.Second==1).ToList());   // ENTERED SECOND TIME

            Save();
        }
        
        public Header Header { get; set; }
        public DataBlock DataAllIn { get; set; }
        public DataBlock DataFirstIn { get; set; }
        public DataBlock DataLastIn { get;set; }


        public void Save()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(MonthOutput));
            using (FileStream fs = new FileStream("out/222.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, this);
            }
        }

        private DataBlock calculate(ICollection<Movebook> set)
        {
            /*  create block All In */
            DataBlock a = new DataBlock();
            a.ValueA = set.Where(x => x.Ai == 1).Count();
            a.ValueB = set.Where(x => x.Bi == 1).Count();
            a.ValueC = set.Where(x => x.Ci == 1).Count();
            a.ValueD = set.Where(x => x.Di == 1).Count();
            a.ValueE = set.Where(x => x.Ei == 1).Count();
            return a;
            
        }
    }
}
