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
        private readonly WebBrowserAdapter _browserAdapter;
        public ICommand UpdateParameters { get; set; }

        public DisplayReport()
        {
            InitializeComponent();
            DataContext = this;

            // Создаем адаптер для старого WebBrowser
            _browserAdapter = new WebBrowserAdapter(Browser);

            // Создаем калькулятор с адаптером
            _calculator = new ReportCalculator(_browserAdapter);

            UpdateParameters = new RelayCommandAsync(_calculator.SetParameters);

            LoadReportList();

            // перехватывает навигацию из HTML в параметре е - Uri
            Browser.Navigating += (a, e) =>
            {
                if (e.Uri != null)
                    LoadReport(e.Uri?.LocalPath ?? string.Empty);
            };
        }

        private async void LoadReport(string reportClassFile)
        {
            try
            {
                // Вычисляем данные через адаптер
                await _calculator.CalculateAsync(reportClassFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_calculator.CurrentReportPath))
            {
                LoadReport(_calculator.CurrentReportPath);
            }
            else
            {
                LoadReportList();
            }
        }

        /// <summary>
        /// Загружает список доступных отчетов
        /// </summary>
        private async void LoadReportList()
        {
            try
            {
                var html = File.ReadAllText("rep/SummaryReports.html");
                await _browserAdapter.NavigateToStringAsync(html);
            }
            catch (Exception ex)
            {
                await _browserAdapter.NavigateToStringAsync($@"
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