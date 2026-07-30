using Castor.gui.common;
using Castor.gui.reports;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Castor.gui.reports
{
    /// <summary>
    /// Логика взаимодействия для DisplayReport.xaml
    /// </summary>
    public partial class DisplayReport : Page, IStartablePage
    {
        public bool CanStart => true;
        private readonly ReportCalculator _calculator;
        public ICommand UpdateParameters { get; set; }

        public DisplayReport()
        {
            InitializeComponent();
            DataContext = this;
            LoadReportList();
            _calculator = new(Browser);

            // перехватывает навигацию из HTML в параметре е - Uri
            Browser.Navigating += (a, e) =>
            {
                if(e.Uri!=null)
                    LoadReport(e.Uri?.LocalPath ?? string.Empty);
            };

            
            UpdateParameters = new RelayCommandAsync(_calculator.SetParameters);
            //LoadReport();
        }

        

        private async void LoadReport(string reportClassFile)
        {
            try
            {
                // Вычисляем данные
                await _calculator.CalculateAsync(reportClassFile);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Загружает список доступных отчетов
        /// </summary>
        private void LoadReportList()
        {
            try
            {
                Browser.NavigateToString(File.ReadAllText("rep/ReportsList.html"));
            }
            catch (Exception ex)
            {
                Browser.NavigateToString($@"
                <html>
                <head><style>body{{font-family:'Segoe UI',sans-serif;display:flex;justify-content:center;align-items:center;height:100vh;background:#f8f9fa;margin:0;}}</style></head>
                <body>
                    <div style='text-align:center;color:#dc3545;'>
                        <h1>❌ Ошибка загрузки списка</h1>
                        <p>{ex.Message}</p>
                    </div>
                </body>
                </html>");
            }
        }

    }
}