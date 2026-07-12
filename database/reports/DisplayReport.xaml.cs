using Castor.database.reports;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using SelectPdf;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace Castor.database.reports

{
    /// <summary>
    /// Логика взаимодействия для DisplayReport.xaml
    /// </summary>
    public partial class DisplayReport : Page, IStartablePage
    {
        public bool CanStart => true;

        //private PdfDocument pdfDocument;
        public DisplayReport(ICastorHtmlReport _report)
        {
            InitializeComponent();

            // show report 
            //webBrowser.Language = System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            
            
            Browser.NavigateToString(_report?.HtmlReport ?? $"NO DATA!");

            
            //HtmlToPdf converter = new HtmlToPdf();
            //converter.Options.PdfPageSize = PdfPageSize.A4;
            //converter.Options.MarginBottom = 25;
            //converter.Options.MarginLeft = 25;
            //converter.Options.MarginRight = 25;
            //converter.Options.MarginTop = 25;
            //pdfDocument = converter.ConvertHtmlString(_report.HtmlReport);
            
        }

        private void SaveToDisk(object sender, RoutedEventArgs e)
        {
            //pdfDocument.Save("out/test.pdf");
            //pdfDocument.Close();

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

        private void BrowseBack(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ReselectPeriod(object sender, RoutedEventArgs e)
        {
            DatePeriod datePeriod = SelectDatePeriod.Show();
            MonthReportHtml monthReportHtml = new MonthReportHtml(datePeriod);
            Browser.NavigateToString(monthReportHtml.HtmlReport);
        }

        public void SaveOnCloseApplication()
        {
        }
    }
}
