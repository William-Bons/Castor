using Castor.database;
using Castor.database.tables;
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

                //select DISORDERED
                ICollection<Movebook> disorders = context.Movebooks.Where(b => b.Dateout >= DateOnly.FromDateTime(datePeriod.Start) && b.Dateout <= DateOnly.FromDateTime(datePeriod.End)).ToList();


                // create header of report

                _MainStringBuilding.AppendFormat(Properties.ResourceRu.MonthReport,
                    string.Format(Properties.ResourceRu.MonthReportHeaderString, DateOnly.FromDateTime(datePeriod.Start), DateOnly.FromDateTime(datePeriod.End)),
                    create1(movebooks, "Поступило всего"),
                    create1(movebooks.Where(x => x.First == true).ToList(), "Поступило впервые в жизни"),
                    create1(movebooks.Where(x => x.Second == true).ToList(), "Поступило повторно в году"),
                    create2(disorders, "Выбыло всего")
                    );

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
        private string create1(ICollection<Movebook> movebooks, string blockHeader)
        {
            var sb = new StringBuilder();

            // Add Data Rows
            sb.AppendFormat(Properties.ResourceRu.BlockTrTd,
                blockHeader,
                movebooks.Count(),
                movebooks.Where(x => x.Ai == 1).Count(),
                movebooks.Where(x => x.Bi == 1).Count(),
                movebooks.Where(x => x.Ci == 1).Count(),
                movebooks.Where(x => x.Di == 1).Count(),
                movebooks.Where(x => x.Ei == 1).Count(),
                movebooks.Where(x => x.Fi == 1).Count());

            return sb.ToString();
        }

        /// <summary>
        /// builg table from come OUT patients
        /// </summary>
        /// <param name="movebooks"></param>
        /// <param name="blockHeader"></param>
        /// <returns></returns>
        private string create2(ICollection<Movebook> movebooks, string blockHeader)
        {
            var sb = new StringBuilder();

            // Add Data Rows
            sb.AppendFormat(Properties.ResourceRu.BlockTrTd,
                blockHeader,
                movebooks.Count(),
                movebooks.Where(x => x.Ao == 1).Count(),
                movebooks.Where(x => x.Bo == 1).Count(),
                movebooks.Where(x => x.Co == 1).Count(),
                movebooks.Where(x => x.Do == 1).Count(),
                movebooks.Where(x => x.Eo == 1).Count(),
                movebooks.Where(x => x.Fo == 1).Count());

            return sb.ToString();
        }
    }
}
