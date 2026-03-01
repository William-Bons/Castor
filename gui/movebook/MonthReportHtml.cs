using Castor.database;
using Castor.database.tables;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Castor.gui.movebook
{
    public class MonthReportHtml
    {
        StringBuilder _MainStringBuilding = new StringBuilder();
        Reports _CreatedReport = new Reports();


        public MonthReportHtml() { }
        public MonthReportHtml(DatePeriod datePeriod)
        {
            using (CastorContext context = new CastorContext())
            {
                // select movelines from Movebook in Date Period ORDERED
                ICollection<Movebook> movebooks = context.Movebooks.Where(b => b.Datein >= DateOnly.FromDateTime(datePeriod.Start) && b.Datein <= DateOnly.FromDateTime(datePeriod.End)).ToList();
                //ICollection<Movebook> moveChild = context.Movebooks.Where(b => b.Datein >= DateOnly.FromDateTime(datePeriod.Start) && b.Datein <= DateOnly.FromDateTime(datePeriod.End) && b.Agein < 18).ToList();

                //select DISORDERED
                ICollection<Movebook> disorders = context.Movebooks.Where(b => b.Dateout >= DateOnly.FromDateTime(datePeriod.Start) && b.Dateout <= DateOnly.FromDateTime(datePeriod.End)).ToList();
                //ICollection<Movebook> disoChild = context.Movebooks.Where(b => b.Dateout >= DateOnly.FromDateTime(datePeriod.Start) && b.Dateout <= DateOnly.FromDateTime(datePeriod.End) && b.Ageout<18).ToList();


                // create error check
                if (movebooks.Where(x => x.InControl==false).Count() > 0)
                {
                    _MainStringBuilding.AppendFormat($"{Properties.ResourceRu.MonthReportInError}");

                    foreach (var item in movebooks.Where(x => x.InControl == false).ToList())
                    {
                        _MainStringBuilding.AppendFormat($"<p>{{0}} {{1}}</p>", item.Fio, item.Dsin);
                    }
                }
                if (disorders.Where(x => x.OutControl == false).Count() > 0)
                {
                    _MainStringBuilding.AppendFormat($"{Properties.ResourceRu.MonthReportOutError}");
                    foreach (var item in disorders.Where(x => x.OutControl == false).ToList())
                    {
                        _MainStringBuilding.AppendFormat($"<p>{{0}} {{1}}</p>", item.Fio, item.Dsout);
                    }
                }

                // create header of report
                _MainStringBuilding.AppendFormat(Properties.ResourceRu.MonthReport,
                    string.Format(Properties.ResourceRu.MonthReportHeaderString, DateOnly.FromDateTime(datePeriod.Start), DateOnly.FromDateTime(datePeriod.End)),
                    create1(movebooks,movebooks.Where(m => m.Agein<18).ToList(), "Поступило всего"),
                    create1(movebooks.Where(x => x.First == true).ToList(), movebooks.Where(x => x.First && x.Agein<18).ToList(), "Поступило впервые в жизни"),
                    create1(movebooks.Where(x => x.Second == true).ToList(), movebooks.Where(x => x.Second && x.Agein<18).ToList(), "Поступило повторно в году"),
                    create2(disorders, disorders.Where(d => d.Ageout<18).ToList(), "Выбыло всего")
                    );

                // save report html into database
                _CreatedReport.Id = 0;
                _CreatedReport.Created = DateTime.Now;
                _CreatedReport.DateStart = DateOnly.FromDateTime(datePeriod.Start);
                _CreatedReport.DateEnd = DateOnly.FromDateTime(datePeriod.End);
                _CreatedReport.ReportName = this.GetType().FullName;
                _CreatedReport.ReportData = Encoding.UTF8.GetBytes(_MainStringBuilding.ToString());
                
                context.Reports.Update(_CreatedReport);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// write report to disk
        /// </summary>
        /// <param name="path">Filename with path</param>
        public void WriteReportToDisk(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (TextWriter writer = new StreamWriter(fs))
                {
                    writer.Write(_MainStringBuilding);
                }
            }
        }

        /// <summary>
        /// Shows Report in dialog as simple formatted HTML 
        /// </summary>
        public void DisplayReportAsHTML()
        {
            using(CastorContext context = new CastorContext())
            {
                _CreatedReport.Printed = true;
                context.Reports.Update(_CreatedReport);
                context.SaveChanges();
            }

            DisplayReport displayReport = new DisplayReport(_MainStringBuilding);
            displayReport.ShowDialog();
        }

        /// <summary>
        /// Build table in html from come IN patients
        /// </summary>
        /// <param name="movebooks"></param>
        /// <param name="blockHeader"></param>
        /// <returns></returns>
        private string create1(ICollection<Movebook> movebooks, ICollection<Movebook> children, string blockHeader)
        {
            var sb = new StringBuilder();

            // Add Data Rows
            sb.AppendFormat(Properties.ResourceRu.BlockTrTd,
                blockHeader,
                movebooks.Count(),
                movebooks.Where(x => x.calc0(x.Dsin)[0]).Count(),
                movebooks.Where(x => x.calc0(x.Dsin)[1]).Count(),
                movebooks.Where(x => x.calc0(x.Dsin)[2]).Count(),
                movebooks.Where(x => x.calc0(x.Dsin)[3]).Count(),
                movebooks.Where(x => x.calc0(x.Dsin)[4]).Count(),
                movebooks.Where(x => x.calc0(x.Dsin)[5]).Count(),
                movebooks.Where(x => x.calc0(x.Dsin)[6]).Count(),

                children.Count(),
                children.Where(x => x.calc0(x.Dsin)[0]).Count(),
                children.Where(x => x.calc0(x.Dsin)[1]).Count(),
                children.Where(x => x.calc0(x.Dsin)[2]).Count(),
                children.Where(x => x.calc0(x.Dsin)[3]).Count(),
                children.Where(x => x.calc0(x.Dsin)[4]).Count(),
                children.Where(x => x.calc0(x.Dsin)[5]).Count(),
                children.Where(x => x.calc0(x.Dsin)[6]).Count());

            return sb.ToString();
        }

        /// <summary>
        /// builg table from come OUT patients
        /// </summary>
        /// <param name="movebooks"></param>
        /// <param name="blockHeader"></param>
        /// <returns></returns>
        private string create2(ICollection<Movebook> movebooks, ICollection<Movebook> children, string blockHeader)
        {
            var sb = new StringBuilder();

            // Add Data Rows
            sb.AppendFormat(Properties.ResourceRu.BlockTrTd,
                blockHeader,
                movebooks.Count(),
                movebooks.Where(x => x.calc0(x.Dsout)[0]).Count(),
                movebooks.Where(x => x.calc0(x.Dsout)[1]).Count(),
                movebooks.Where(x => x.calc0(x.Dsout)[2]).Count(),
                movebooks.Where(x => x.calc0(x.Dsout)[3]).Count(),
                movebooks.Where(x => x.calc0(x.Dsout)[4]).Count(),
                movebooks.Where(x => x.calc0(x.Dsout)[5]).Count(),
                movebooks.Where(x => x.calc0(x.Dsout)[6]).Count(),

                children.Count(),
                children.Where(x => x.calc0(x.Dsout)[0]).Count(),
                children.Where(x => x.calc0(x.Dsout)[1]).Count(),
                children.Where(x => x.calc0(x.Dsout)[2]).Count(),
                children.Where(x => x.calc0(x.Dsout)[3]).Count(),
                children.Where(x => x.calc0(x.Dsout)[4]).Count(),
                children.Where(x => x.calc0(x.Dsout)[5]).Count(),
                children.Where(x => x.calc0(x.Dsout)[6]).Count());

            return sb.ToString();
        }
    }
}
