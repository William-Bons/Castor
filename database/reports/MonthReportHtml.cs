using Castor.database.tables;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Castor.database.reports
{
    public class MonthReportHtml : ICastorHtmlReport, INotifyPropertyChanged
    {
        StringBuilder _MainStringBuilding = new StringBuilder();

        public event PropertyChangedEventHandler? PropertyChanged;

        public string HtmlReport => _MainStringBuilding.ToString();
        public DatePeriod datePeriod { get; set; } = new DatePeriod();
        public string ReportTitle => $"Движение {datePeriod.Start:d} - {datePeriod.End:d}";

        public MonthReportHtml()
        {
        }

        public void Calculate()
        {
            using CastorContext context = new CastorContext();
            // select movelines from PatientRecord in Date Period ORDERED
            IEnumerable<Movebook> movebooks = context.Movebooks
                .Where(b => b.Datein >= DateOnly.FromDateTime(datePeriod.Start) && b.Datein <= DateOnly.FromDateTime(datePeriod.End))
                .ToList();

            //select DISORDERED in date period
            IEnumerable<Movebook> disorders = context.Movebooks
                .Where(b => b.Dateout >= DateOnly.FromDateTime(datePeriod.Start) && b.Dateout <= DateOnly.FromDateTime(datePeriod.End))
                .ToList();

            // select patients in DEP NOW
            IEnumerable<Movebook> ordered = context.Movebooks
                .Where(o => o.Datein.HasValue && !o.Dateout.HasValue)
                .Include(o => o.Forceds);

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
                create3(ordered.Where(m => m.Forceds.Any()), ordered.Where(m => m.Forceds.Any() && m.Agein < 18), "На принудительном лечении"),
                create3(ordered.Where(m => m.Forceds.Any() && m.DaysToday > 365), ordered.Where(m => m.Forceds.Any() && m.Agein < 18 && m.DaysToday > 365), " - из них более года"),

                create4("Поступило в порядке НГ"), //todo create3(movebooks.Where(m => m.Unvoluntaryid > 0), movebooks.Where(m => m.Unvoluntaryid > 0 && m.Agein < 18), "Поступило в порядке НГ"),
                create4("-из них по решению суда"), //todo create3(movebooks.Where(m => m.Unvoluntaryid > 0), movebooks.Where(m => m.Unvoluntaryid > 0 && m.Agein < 18), " - из них по решению суда"),

                create4("Оформлено документов МСЭ, ИПР, восстановление справок"),
                create4("Наличие  ЧП,  побеги")

                );
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReportTitle)));
        }

        /// <summary>
        /// Build table in html from come IN patients
        /// </summary>
        /// <param name="movebooks"></param>
        /// <param name="blockHeader"></param>
        /// <returns></returns>
        private string create1(IEnumerable<Movebook> movebooks, IEnumerable<Movebook> children, string blockHeader)
        {
            try
            {
                var sb = new StringBuilder();

                // Add Data Rows
                sb.AppendFormat(Properties.ResourceRu.BlockTrTd,
                    blockHeader,
                    movebooks.Count(),
                    movebooks.Count(x => x.calc0(x.Dsin)[0]),
                    movebooks.Count(x => x.calc0(x.Dsin)[1]),
                    movebooks.Count(x => x.calc0(x.Dsin)[2]),
                    movebooks.Count(x => x.calc0(x.Dsin)[3]),
                    movebooks.Count(x => x.calc0(x.Dsin)[4]),
                    movebooks.Count(x => x.calc0(x.Dsin)[5]),
                    movebooks.Count(x => x.calc0(x.Dsin)[6]),

                    children.Count(),
                    children.Count(x => x.calc0(x.Dsin)[0]),
                    children.Count(x => x.calc0(x.Dsin)[1]),
                    children.Count(x => x.calc0(x.Dsin)[2]),
                    children.Count(x => x.calc0(x.Dsin)[3]),
                    children.Count(x => x.calc0(x.Dsin)[4]),
                    children.Count(x => x.calc0(x.Dsin)[5]),
                    children.Count(x => x.calc0(x.Dsin)[6]));

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return $"<tr><td> create1 Error </td></tr>";
            }

        }

        /// <summary>
        /// builg table from come OUT patients
        /// </summary>
        /// <param name="movebooks"></param>
        /// <param name="blockHeader"></param>
        /// <returns></returns>
        private string create2(IEnumerable<Movebook> movebooks, IEnumerable<Movebook> children, string blockHeader)
        {
            try
            {
                var sb = new StringBuilder();

                // Add Data Rows
                sb.AppendFormat(Properties.ResourceRu.BlockTrTd,
                    blockHeader,
                    movebooks.Count(),
                    movebooks.Count(x => x.calc0(x.Dsout)[0]),
                    movebooks.Count(x => x.calc0(x.Dsout)[1]),
                    movebooks.Count(x => x.calc0(x.Dsout)[2]),
                    movebooks.Count(x => x.calc0(x.Dsout)[3]),
                    movebooks.Count(x => x.calc0(x.Dsout)[4]),
                    movebooks.Count(x => x.calc0(x.Dsout)[5]),
                    movebooks.Count(x => x.calc0(x.Dsout)[6]),

                    children.Count(),
                    children.Count(x => x.calc0(x.Dsout)[0]),
                    children.Count(x => x.calc0(x.Dsout)[1]),
                    children.Count(x => x.calc0(x.Dsout)[2]),
                    children.Count(x => x.calc0(x.Dsout)[3]),
                    children.Count(x => x.calc0(x.Dsout)[4]),
                    children.Count(x => x.calc0(x.Dsout)[5]),
                    children.Count(x => x.calc0(x.Dsout)[6]));

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return $"<tr><td> create2 Error </td></tr>";
            }
        }

        private string create3(IEnumerable<Movebook> movebooks, IEnumerable<Movebook> children, string blockHeader)
        {
            try
            {
                var sb = new StringBuilder();

                // Add Data Rows
                sb.AppendFormat(Properties.ResourceRu.BlockTrLine,
                    blockHeader,
                    movebooks.Count(),
                    children.Count());

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return $"<tr><td> create3 Error </td></tr>";
            }
        }

        private string create4(string blockHeader)
        {
            try
            {
                var sb = new StringBuilder();

                // Add Data Rows
                sb.AppendFormat(Properties.ResourceRu.BlockTrLine,
                    blockHeader,
                    0,
                    0);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return $"<tr><td> create4 Error </td></tr>";
            }
        }


    }
}
