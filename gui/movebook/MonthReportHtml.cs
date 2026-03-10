using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace Castor.gui.movebook
{
    public class MonthReportHtml : ICastorHtmlReport
    {
        StringBuilder _MainStringBuilding = new StringBuilder();
        Reports _CreatedReport = new Reports();

        public string HtmlReport => _MainStringBuilding.ToString();
        public DatePeriod datePeriod {  get; set; }

        //public MonthReportHtml() { }
        public MonthReportHtml(DatePeriod datePeriod)
        {
            this.datePeriod = datePeriod;
            Calculate();
        }

        public void Calculate()
        {
            using (CastorContext context = new CastorContext())
            {
                // select movelines from Movebook in Date Period ORDERED
                IEnumerable<Movebook> movebooks = context.Movebooks.Where(b => b.Datein >= DateOnly.FromDateTime(datePeriod.Start) && b.Datein <= DateOnly.FromDateTime(datePeriod.End)).ToList();

                //select DISORDERED in date period
                IEnumerable<Movebook> disorders = context.Movebooks.Where(b => b.Dateout >= DateOnly.FromDateTime(datePeriod.Start) && b.Dateout <= DateOnly.FromDateTime(datePeriod.End)).ToList();

                // select patients in DEP NOW
                IEnumerable<Movebook> ordered = context.Movebooks.Where(o => o.Datein.HasValue && !o.Dateout.HasValue);

                // create error check
                if (movebooks.Where(x => x.InControl == false).Count() > 0)
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

                // create BODY of report
                _MainStringBuilding.AppendFormat(Properties.ResourceRu.MonthReport,
                    string.Format(Properties.ResourceRu.MonthReportHeaderString, DateOnly.FromDateTime(datePeriod.Start), DateOnly.FromDateTime(datePeriod.End)),
                    create1(movebooks, movebooks.Where(m => m.Agein < 18), "Поступило всего"),
                    create1(movebooks.Where(x => x.First == true), movebooks.Where(x => x.First && x.Agein < 18), "Поступило впервые в жизни"),
                    create1(movebooks.Where(x => x.Second == true), movebooks.Where(x => x.Second && x.Agein < 18), "Поступило повторно в году"),
                    create2(disorders, disorders.Where(d => d.Ageout < 18), "Выбыло всего"),
                    create3(disorders.Where(d => d.Deceased), disorders.Where(d => d.Deceased && d.Ageout < 18), "Умерло"),
                    create3(disorders.Where(m => m.Outto == 3), disorders.Where(m => m.Outto == 3 && m.Agein < 18), "Перевод в ПНИ"),

                    create3(ordered.Where(m => m.DaysToday > 365), ordered.Where(m => m.DaysToday > 365 && m.Agein < 18), "Кол-во хронизированных"),
                    create3(ordered.Where(m => m.Forced.HasValue), ordered.Where(m => m.Forced.HasValue && m.Agein < 18), "На принудительном лечении"),
                    create3(ordered.Where(m => m.Forced.HasValue && m.DaysToday > 365), ordered.Where(m => m.Forced.HasValue && m.Agein < 18 && m.DaysToday > 365), " - из них более года"),

                    create3(movebooks.Where(m => m.Unvoluntary), movebooks.Where(m => m.Unvoluntary && m.Agein < 18), "Поступило в порядке НГ"),
                    create3(movebooks.Where(m => m.Unvoluntary), movebooks.Where(m => m.Unvoluntary && m.Agein < 18), " - из них по решению суда"),

                    create4("Оформлено документов МСЭ, ИПР, восстановление справок"),
                    create4("Наличие  ЧП,  побеги")

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
        /// Shows Report in dialog as simple formatted HTML 
        /// </summary>
        public Page DisplayReportAsHTML()
        {
            /* записывет отчет в таблицу */
            using(CastorContext context = new CastorContext())
            {
                _CreatedReport.Printed = true;
                context.Reports.Update(_CreatedReport);
                context.SaveChanges();
            }

            DisplayReport displayReport = new DisplayReport(this);
            return displayReport;
        }

        /// <summary>
        /// Build table in html from come IN patients
        /// </summary>
        /// <param name="movebooks"></param>
        /// <param name="blockHeader"></param>
        /// <returns></returns>
        private string create1(IEnumerable<Movebook> movebooks, IEnumerable<Movebook> children, string blockHeader)
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
        private string create2(IEnumerable<Movebook> movebooks, IEnumerable<Movebook> children, string blockHeader)
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

        private string create3(IEnumerable<Movebook> movebooks, IEnumerable<Movebook> children, string blockHeader)
        {
            var sb = new StringBuilder();

            // Add Data Rows
            sb.AppendFormat(Properties.ResourceRu.BlockTrLine,
                blockHeader,
                movebooks.Count(),
                children.Count());
                
            return sb.ToString();
        }

        private string create4(string blockHeader)
        {
            var sb = new StringBuilder();

            // Add Data Rows
            sb.AppendFormat(Properties.ResourceRu.BlockTrLine,
                blockHeader,
                0,
                0);

            return sb.ToString();
        }

        
    }
}
