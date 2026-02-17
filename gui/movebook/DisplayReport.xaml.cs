using SelectPdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для DisplayReport.xaml
    /// </summary>
    public partial class DisplayReport : Window
    {
        private PdfDocument pdfDocument;
        public DisplayReport(MonthReportHtml monthReportHtml)
        {
            InitializeComponent();


            // show report 
            //webBrowser.Language = System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            Browser.NavigateToString(monthReportHtml.MainString.ToString());

            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.MarginBottom = 25;
            converter.Options.MarginLeft = 25;
            converter.Options.MarginRight = 25;
            converter.Options.MarginTop = 25;
            pdfDocument = converter.ConvertHtmlString(monthReportHtml.MainString.ToString());
        }

        private void SaveToDisk(object sender, RoutedEventArgs e)
        {
            pdfDocument.Save("out/test.pdf");
            pdfDocument.Close();

            // show report 
            Window w = new Window();
            WebBrowser webBrowser = new WebBrowser();
            webBrowser.Language = System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            webBrowser.Navigate(Path.GetFullPath("out/test.pdf"));
            w.Content = webBrowser;
            w.Show();
        }

        private void Print(object sender, RoutedEventArgs e)
        {

        }
    }
}
