using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Castor.database.reports
{
    /// <summary>
    /// Логика взаимодействия для DisplayReport.xaml
    /// </summary>
    public partial class DisplayReport : Page, IStartablePage
    {
        public bool CanStart => true;
        private readonly ReportCalculator _calculator;

        public DisplayReport(object reportClassName = null)
        {
            InitializeComponent();

            // Настраиваем WebBrowser для взаимодействия с JavaScript
            //Browser.ObjectForScripting = this;
            //Browser.Navigating += Browser_Navigating;

            //if (reportClassName != null && !string.IsNullOrEmpty(reportClassName.ToString()))
            //{
            //    LoadReport(reportClassName.ToString());
            //}
            //else
            //{
            //    LoadReportList();
            //}
            _calculator = new ReportCalculator();
            LoadReport();
        }


        private async void LoadReport()
        {
            try
            {
                _calculator.SetParameter("StartDate", DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"));
                _calculator.SetParameter("EndDate", DateTime.Now.ToString("yyyy-MM-dd"));

                // Вычисляем данные
                var (data, period, department) = await _calculator.CalculateAsync(@"assets\PatientMovementReport.html");

                // Загружаем HTML
                var html = File.ReadAllText(GetFullPath(@"assets\PatientMovementReport.html"));
                Browser.NavigateToString(html);

                // Ждем загрузки и вызываем функцию updateReport
                await Task.Delay(500);

                var dataJson = JsonConvert.SerializeObject(data);
                var script = $"updateReport({dataJson}, '{period}', '{department}');";
                Browser.InvokeScript("eval", script);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private string GetFullPath(string relativePath)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(baseDirectory, relativePath);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        /// <summary>
        /// Загружает список доступных отчетов
        /// </summary>
        private void LoadReportList()
        {
            try
            {
                Browser.NavigateToString(File.ReadAllText("assets/ReportsList.html"));
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

        /// <summary>
        /// Метод для вызова из JavaScript
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(true)]
        public void SelectReport(string reportClassName)
        {
            // Используем Dispatcher для выполнения в UI потоке
            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(reportClassName))
                {
                    // Загружаем выбранный отчет
                    //LoadReport(reportClassName);
                }
            });
        }

        /// <summary>
        /// Обработчик навигации для перехвата кликов по ссылкам
        /// </summary>
        private void Browser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            // Если URL начинается с "report:", извлекаем имя класса
            if (e.Uri != null && e.Uri.ToString().StartsWith("report:"))
            {
                e.Cancel = true;
                string reportClassName = e.Uri.ToString().Substring(7); // Убираем "report:"
                SelectReport(reportClassName);
            }
        }
    }
}