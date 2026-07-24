using Castor.database;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.gui.login;
using Castor.gui.movebook;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Castor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // ЭТО САМОЕ ВАЖНОЕ: ловим ошибку ДО того, как WPF начнёт падать
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Используем твой централизованный метод логирования
            LogError("Startup", e.Exception);

            string userMessage = $"Критическая ошибка при старте приложения.\n\nПричина: {e.Exception.Message}\n\nПодробности записаны в лог-файл в папке Logs.\n\nОбратитесь к системному администратору.";

            MessageBox.Show(
                userMessage,
                "Ошибка запуска Castor",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );

            e.Handled = true;
            // Никакого Shutdown() — приложение корректно завершит инициализацию
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if RESET
            Debug.WriteLine("🔄 ВЫПОЛНЯЕТСЯ СБРОС НАСТРОЕК (DebugReset)");
            Settings.Default.Reset();
            Settings.Default.Save();
#endif

            // 1. Проверка наличия зашифрованной строки подключения к Medis, если нет - запрос строки
            string connString = Settings.Default.postgreeConnection;
            if (string.IsNullOrWhiteSpace(connString))
            {
                var dialog = new ConnectionDialog();
                if (dialog.ShowDialog() != true)
                {
                    // Пользователь нажал "Отмена" при вводе данных
                    Application.Current.Shutdown();
                    return;
                }
                connString = dialog.ConnectionString;
            }

            // проверка сущестования файла БД для текущего отделения
            if (!File.Exists(Settings.Default.sqliteConnection))
            {
                // если file не существует, запрос отделения и пользователя
                new SelectUser().ShowDialog();
            }

            // бэкап только если успешны проверка и update
            using CastorContext castorContext = new CastorContext();
            castorContext.Backup();

            // Миграции
            try
            {
                castorContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                LogError("Migrations", ex);
                MessageBox.Show($"Ошибка обновления базы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Синхронизация базы
            Task.Run(() => new Synchronization().LoadExistsFromMedis());


            // Загружаем сохранённую тему перед отображением главного окна
            ThemeManager.LoadSavedTheme();

            // Полноценный вход: показываем обычный MainWindow
            var mainWindow = new MainWindow();
            mainWindow.Title = "Castor — Режим администратора";
            mainWindow.Show();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                // Логируем состояние перед выходом
                var count = ConnectionMonitorManager.Instance.ActiveConnectionsCount;
                var contexts = ConnectionMonitorManager.Instance.GetActiveContexts();
                var contextNames = string.Join(", ", contexts.Select(c => c.GetType().Name));

                Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] ========================================");
                Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] 🚪 ВЫХОД ИЗ ПРИЛОЖЕНИЯ");
                Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] 📊 Активных соединений: {count}");

                if (count > 0)
                {
                    Debug.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss.fff}] 📋 Активные контексты: {contextNames}");
                    Debug.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss.fff}] ⚠️ ВНИМАНИЕ: Остались незакрытые соединения!");

                    // Закрываем соединения принудительно
                    ConnectionMonitorManager.SafeCloseAllConnections();
                }
                else
                {
                    Debug.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss.fff}] ✅ Все соединения закрыты корректно");
                }

                Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] ========================================");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] ❌ Ошибка при выходе: {ex.Message}");
                Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] StackTrace: {ex.StackTrace}");
            }

            base.OnExit(e);
        }
        /// <summary>
        /// при необходимости вызывается эта процедура для установления пользователя и проверки пароля
        /// </summary>
        /// <returns></returns>
        private AuthResult TryAuthenticate()
        {
            // Создаём окно входа БЕЗ контекста БД (оно просто собирает данные)
            var loginWindow = new LoginWindow();

            // Опционально: если хочешь предварительно загрузить список пользователей в ComboBox
            // Это можно сделать через временный контекст, но лучше пусть пользователь вводит логин сам.

            var dialogResult = loginWindow.ShowDialog();

            // Пользователь нажал "Отмена" или крестик
            if (dialogResult == false)
            {
                return new AuthResult
                {
                    IsCancelled = true,
                    ErrorMessage = "Вход отменён"
                };
            }

            string login = loginWindow.GetLogin();
            string password = loginWindow.PasswordBox.Password;

            // ВАЖНО: Очищаем память от пароля сразу после получения
            loginWindow.PasswordBox.Clear();

            // Используем наш сервис для проверки
            var authService = new AuthService();

            if (!authService.ValidateCredentials(login, password))
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Неверный логин или пароль"
                };
            }

            return new AuthResult { IsSuccess = true };
        }


        // --- НОВЫЕ МЕТОДЫ ЛОГИРОВАНИЯ (вставь их в класс App) ---
        public static void LogError(string category, Exception ex)
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDir);
            var path = Path.Combine(logDir, $"error_{category}_{DateTime.Now:yyyyMMdd}.log");

            var message = $"[{DateTime.Now:o}] [{category}] {ex.ToString()}\n\n";
            File.AppendAllText(path, message);
        }

        public static void LogWarning(string category, string message)
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(logDir);
            var path = Path.Combine(logDir, $"warn_{category}_{DateTime.Now:yyyyMMdd}.log");

            var line = $"[{DateTime.Now:o}] [{category}] {message}\n";
            File.AppendAllText(path, line);
        }
        // ---------------------------------------------------------

    }



    public class AuthResult
    {
        public bool IsSuccess { get; set; }
        public bool IsCancelled { get; set; } // пользователь нажал «Отмена»
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
