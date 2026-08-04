using Castor.gui.common;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.reports
{
    /// <summary>
    /// Новая страница отображения отчетов на основе WebView2
    /// Поддерживает печать и предварительный просмотр
    /// </summary>
    public partial class DisplayReportWebView2 : Page, IStartablePage
    {
        public bool CanStart => true;

        private readonly ReportCalculator _calculator;
        private readonly WebView2BrowserAdapter _browserAdapter;
        private bool _isBrowserInitialized = false;

        public ICommand UpdateParameters { get; set; }

        public DisplayReportWebView2()
        {
            InitializeComponent();
            DataContext = this;

            // Создаем адаптер для WebView2
            _browserAdapter = new WebView2BrowserAdapter(Browser);

            // Создаем калькулятор с адаптером
            _calculator = new ReportCalculator(_browserAdapter);

            // Команда обновления параметров
            UpdateParameters = new RelayCommandAsync(_calculator.SetParameters);

            // Загружаем список отчетов
            LoadReportList();
        }

        /// <summary>
        /// Инициализация при загрузке страницы
        /// </summary>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeBrowserAsync();
        }

        /// <summary>
        /// Асинхронная инициализация WebView2
        /// </summary>
        private async Task InitializeBrowserAsync()
        {
            if (_isBrowserInitialized)
                return;

            try
            {
                StatusText.Text = "⏳ Инициализация браузера...";

                await Browser.EnsureCoreWebView2Async();

                // Подписываемся на события
                Browser.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
                Browser.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

                _isBrowserInitialized = true;
                StatusText.Text = "✅ Браузер готов";
            }
            catch (Exception ex)
            {
                StatusText.Text = "❌ Ошибка инициализации";

                MessageBox.Show(
                    $"Ошибка инициализации WebView2:\n\n{ex.Message}\n\n" +
                    "Убедитесь, что установлен WebView2 Runtime.\n" +
                    "Скачать: https://go.microsoft.com/fwlink/p/?LinkId=2124703",
                    "Ошибка браузера",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Browser.NavigateToString(GetErrorHtml("Ошибка инициализации браузера", ex.Message));
            }
        }

        /// <summary>
        /// Обработка открытия новых окон
        /// </summary>
        private void OnNewWindowRequested(object? sender,
            Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            Browser.CoreWebView2.Navigate(e.Uri);
        }

        /// <summary>
        /// Обработка сообщений из JavaScript
        /// </summary>
        private void OnWebMessageReceived(object? sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var message = e.TryGetWebMessageAsString();
            // Можно обрабатывать сообщения из JavaScript
            // Например, для логирования или команд
        }

        /// <summary>
        /// Загрузка отчета
        /// </summary>
        private async void LoadReport(string reportClassFile)
        {
            if (string.IsNullOrEmpty(reportClassFile))
                return;

            try
            {
                StatusText.Text = $"⏳ Загрузка: {Path.GetFileName(reportClassFile)}...";

                // Проверяем инициализацию
                if (!_isBrowserInitialized)
                {
                    await InitializeBrowserAsync();
                }

                // Вычисляем данные через калькулятор
                await _calculator.CalculateAsync(reportClassFile);

                StatusText.Text = $"✅ Отчет загружен: {Path.GetFileName(reportClassFile)}";
            }
            catch (Exception ex)
            {
                StatusText.Text = "❌ Ошибка загрузки отчета";

                MessageBox.Show($"Ошибка загрузки отчета:\n\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Загрузка списка отчетов
        /// </summary>
        private async void LoadReportList()
        {
            try
            {
                var htmlPath = "rep/SummaryReports.html";

                if (!File.Exists(htmlPath))
                {
                    await ShowErrorPage("Файл списка отчетов не найден",
                                       $"Файл '{htmlPath}' отсутствует");
                    return;
                }

                var html = File.ReadAllText(htmlPath);

                // Проверяем инициализацию
                if (!_isBrowserInitialized)
                {
                    await InitializeBrowserAsync();
                }

                // Загружаем HTML
                if (_isBrowserInitialized && Browser.CoreWebView2 != null)
                {
                    Browser.NavigateToString(html);
                    StatusText.Text = "📋 Список отчетов загружен";
                }
                else
                {
                    await ShowErrorPage("Браузер не инициализирован",
                                       "Попробуйте перезагрузить страницу");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorPage("Ошибка загрузки списка", ex.Message);
            }
        }

        /// <summary>
        /// Показать страницу с ошибкой
        /// </summary>
        private Task ShowErrorPage(string title, string message)
        {
            Browser.NavigateToString(GetErrorHtml(title, message));
            StatusText.Text = "❌ Ошибка";
            return Task.CompletedTask;
        }

        /// <summary>
        /// HTML шаблон для ошибок
        /// </summary>
        private string GetErrorHtml(string title, string message)
        {
            return $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: 'Segoe UI', sans-serif;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            height: 100vh;
                            background: #f8f9fa;
                            margin: 0;
                            padding: 20px;
                        }}
                        .error-container {{
                            text-align: center;
                            max-width: 600px;
                            background: white;
                            padding: 40px;
                            border-radius: 8px;
                            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                        }}
                        h1 {{ color: #dc3545; margin-top: 0; }}
                        .message {{ color: #666; margin: 20px 0; line-height: 1.6; }}
                        .hint {{ font-size: 12px; color: #999; margin-top: 20px; }}
                    </style>
                </head>
                <body>
                    <div class='error-container'>
                        <h1>❌ {title}</h1>
                        <div class='message'>{message}</div>
                        <div class='hint'>Используйте кнопку 'Обновить' для повторной попытки</div>
                    </div>
                </body>
                </html>";
        }

        /// <summary>
        /// Обработка выбора отчета из списка
        /// </summary>
        private void ReportList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportList.SelectedItem is string reportPath && !string.IsNullOrEmpty(reportPath))
            {
                LoadReport(reportPath);
            }
        }

        /// <summary>
        /// Обновление отчета
        /// </summary>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_calculator.CurrentReportPath))
            {
                await _calculator.CalculateAsync(_calculator.CurrentReportPath);
                StatusText.Text = "🔄 Отчет обновлен";
            }
            else
            {
                LoadReportList();
            }
        }

        /// <summary>
        /// Печать
        /// </summary>
        private async void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isBrowserInitialized)
                    await InitializeBrowserAsync();

                if (Browser.CoreWebView2 != null)
                {
                    Browser.CoreWebView2.ShowPrintUI();
                    StatusText.Text = "🖨️ Открыт диалог печати";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка печати:\n\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Предварительный просмотр печати
        /// </summary>
        private async void PrintPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isBrowserInitialized)
                    await InitializeBrowserAsync();

                if (Browser.CoreWebView2 != null)
                {
                    // WebView2 использует тот же диалог для печати и предпросмотра
                    Browser.CoreWebView2.ShowPrintUI();
                    StatusText.Text = "📄 Открыт предпросмотр печати";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка предпросмотра:\n\n{ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Экспорт в PDF
        /// </summary>
        //private async void ExportPdfButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (!_isBrowserInitialized)
        //            await InitializeBrowserAsync();

        //        if (Browser.CoreWebView2 == null)
        //            return;

        //        var saveDialog = new Microsoft.Win32.SaveFileDialog
        //        {
        //            Filter = "PDF files (*.pdf)|*.pdf",
        //            DefaultExt = "pdf",
        //            FileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
        //        };

        //        if (saveDialog.ShowDialog() == true)
        //        {
        //            var settings = new Microsoft.Web.WebView2.Core.CoreWebView2PrintSettings
        //            {
        //                ShouldPrintBackgrounds = true,
        //                Orientation = Microsoft.Web.WebView2.Core.CoreWebView2PrintOrientation.Portrait,
        //                PageWidth = 210,  // A4 в мм
        //                PageHeight = 297
        //            };

        //            await Browser.CoreWebView2.PrintToPdfAsync(saveDialog.FileName, settings);
        //            StatusText.Text = $"✅ PDF сохранен: {Path.GetFileName(saveDialog.FileName)}";

        //            MessageBox.Show($"PDF успешно сохранен:\n{saveDialog.FileName}",
        //                          "Успех",
        //                          MessageBoxButton.OK,
        //                          MessageBoxImage.Information);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка экспорта в PDF:\n\n{ex.Message}",
        //                      "Ошибка",
        //                      MessageBoxButton.OK,
        //                      MessageBoxImage.Error);
        //    }
        //}
    }
}