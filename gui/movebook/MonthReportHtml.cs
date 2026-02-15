using Castor.database;
using Castor.database.tables;
using Castor.gui.dialogs;
using SelectPdf;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Castor.gui.movebook
{
    public class MonthReportHtml
    {
        StringBuilder _mainStringBuilding = new StringBuilder();
        public MonthReportHtml() { }
        public MonthReportHtml(DatePeriod datePeriod)
        {
            using (CastorContext context = new CastorContext())
            {
                // select movelines from Movebook in Date Period ORDERED
                ICollection<Movebook> movebooks = context.Movebooks.Where(b => b.Datein >= DateOnly.FromDateTime(datePeriod.Start) && b.Datein <= DateOnly.FromDateTime(datePeriod.End)).ToList();

                //select DISORDERED
                ICollection<Movebook> disorders = context.Movebooks.Where(b => b.Dateout >= DateOnly.FromDateTime(datePeriod.Start) && b.Dateout <= DateOnly.FromDateTime(datePeriod.End)).ToList();

                // create HTML header, style
                _mainStringBuilding.AppendLine($"<!DOCTYPE html ><html><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>");
                _mainStringBuilding.AppendLine($"<head>{Properties.ResourceRu.MonthReportStyle}</head>");
                _mainStringBuilding.AppendLine("<body>");

                // create header of report
                _mainStringBuilding.AppendFormat(Properties.ResourceRu.MonthReportHeaderString, DateOnly.FromDateTime(datePeriod.Start), DateOnly.FromDateTime(datePeriod.End));

                // add table blocks
                _mainStringBuilding.AppendLine(create1(movebooks, "Поступило всего"));
                _mainStringBuilding.AppendLine(create1(movebooks.Where(x => x.First == 1).ToList(), "Поступило впервые в жизни"));
                _mainStringBuilding.AppendLine(create1(movebooks.Where(x => x.Second == 1).ToList(), "Поступило повторно в году"));
                _mainStringBuilding.AppendLine(create1(disorders, "Выбыло всего"));

                // create end of html
                _mainStringBuilding.AppendLine("</body></html>");
            }
        }

        public void WriteToPdf()
        {
            

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.MarginBottom = 25;
            converter.Options.MarginLeft = 25;  
            converter.Options.MarginRight = 25;  
            converter.Options.MarginTop = 25;
            PdfDocument doc = converter.ConvertHtmlString(_mainStringBuilding.ToString());
            doc.Save("out/test.pdf");
            doc.Close();

            // show report 
            Window w = new Window();
            WebBrowser webBrowser = new WebBrowser();
            webBrowser.Language = System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            webBrowser.Navigate(Path.GetFullPath("out/test.pdf"));
            w.Content = webBrowser;
            w.Show();
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
                    writer.Write(_mainStringBuilding);
                }
            }
        }

        /// <summary>
        /// Shows Report in dialog as simple formatted HTML 
        /// </summary>
        public void DisplayReportAsHTML()
        {
            // show report 
            Window w = new Window();
            WebBrowser webBrowser = new WebBrowser();
            webBrowser.Language = System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            webBrowser.NavigateToString(_mainStringBuilding.ToString());
            w.Content = webBrowser;
            w.Show();
        }

        private string create1(ICollection<Movebook> movebooks, string blockHeader)
        {
            var sb = new StringBuilder();

            // Table start and styling
            sb.AppendLine("<table>");
            sb.AppendLine($"<tr><th>{blockHeader}</th><th>{movebooks.Count()}</th></tr>");

            // Add Data Rows
            sb.AppendFormat(Properties.ResourceRu.BlockTrTd,
                movebooks.Where(x => x.Ai == 1).Count(),
                movebooks.Where(x => x.Bi == 1).Count(),
                movebooks.Where(x => x.Ci == 1).Count(),
                movebooks.Where(x => x.Di == 1).Count(),
                movebooks.Where(x => x.Ei == 1).Count());

            // Table end
            sb.Append("</table>");
            return sb.ToString();
        }

    }
}
