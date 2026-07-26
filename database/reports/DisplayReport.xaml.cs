using Castor.gui.common;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows;

namespace Castor.database.reports
{
    /// <summary>
    /// Логика взаимодействия для DisplayReport.xaml
    /// </summary>
    public partial class DisplayReport : Page, IStartablePage
    {
        public bool CanStart => true;

        public DisplayReport(object reportClassName = null)
        {
            InitializeComponent();

            // Настраиваем WebBrowser для взаимодействия с JavaScript
            Browser.ObjectForScripting = this;
            Browser.Navigating += Browser_Navigating;

            if (reportClassName != null && !string.IsNullOrEmpty(reportClassName.ToString()))
            {
                LoadReport(reportClassName.ToString());
            }
            else
            {
                LoadReportList();
            }
        }


        /// <summary>
        /// Загружает конкретный отчет по имени класса
        /// </summary>
        private void LoadReport(string reportClassName)
        {
            try
            {
                // Получаем тип отчета по имени
                Type reportType = Type.GetType(reportClassName);

                if (reportType != null)
                {
                    // Создаем экземпляр отчета
                    var report = (ICastorHtmlReport)Activator.CreateInstance(reportType);

                    // Вычисляем отчет
                    report?.Calculate();

                    // Отображаем HTML отчет
                    string htmlContent = report?.HtmlReport ?? "<h1>Отчет не содержит данных</h1>";
                    Browser.NavigateToString(htmlContent);

                    // Устанавливаем DataContext
                    DataContext = report;
                }
                else
                {
                    Browser.NavigateToString($"<h1>Ошибка: Тип '{reportClassName}' не найден</h1>");
                }
            }
            catch (Exception ex)
            {
                Browser.NavigateToString($@"
                <html>
                <head><style>body{{font-family:'Segoe UI',sans-serif;display:flex;justify-content:center;align-items:center;height:100vh;background:#f8f9fa;margin:0;}}</style></head>
                <body>
                    <div style='text-align:center;color:#dc3545;'>
                        <h1>❌ Ошибка загрузки отчета</h1>
                        <p>{ex.Message}</p>
                        <p style='font-size:12px;color:#6c757d;'>{ex.StackTrace}</p>
                    </div>
                </body>
                </html>");
            }
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
                    LoadReport(reportClassName);
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