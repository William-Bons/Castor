using Castor.database;
using Castor.gui;
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
        private SplashWindow _splashWindow;

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
            // Создаем и показываем Splash
            _splashWindow = new SplashWindow();
            _splashWindow.Show();

            // Запускаем инициализацию в отдельном потоке
            Task.Run(() => InitializeApplication());

        }

        private async Task InitializeApplication()
        {
            try
            {
                // 1. Проверка наличия зашифрованной строки подключения к Medis
                _splashWindow.UpdateProgress("Инициализация базы данных Medis...", 10);

                string connString = Settings.Default.postgreeConnection;
                if (string.IsNullOrWhiteSpace(connString))
                {
                    // Важно: ShowDialog должен выполняться в UI потоке
                    string dialogResult = null;
                    await Dispatcher.InvokeAsync(() =>
                    {
                        var dialog = new ConnectionDialog();
                        if (dialog.ShowDialog() == true)
                        {
                            dialogResult = dialog.ConnectionString;
                        }
                    });

                    if (string.IsNullOrEmpty(dialogResult))
                    {
                        // Пользователь нажал "Отмена"
                        await Dispatcher.InvokeAsync(() => Application.Current.Shutdown());
                        return;
                    }
                    connString = dialogResult;
                }

                // 2. Проверка существования файла БД для текущего отделения
                _splashWindow.UpdateProgress("Инициализация базы данных Castor...", 20);

                if (!File.Exists(Settings.Default.sqliteConnection))
                {
                    // SelectUser должен выполняться в UI потоке
                    await Dispatcher.InvokeAsync(() =>
                    {
                        new SelectUser().ShowDialog();
                    });
                }

                // 3. Бэкап (выполняется синхронно, но в фоновом потоке)
                _splashWindow.UpdateProgress("Backup базы данных...", 40);
                await Task.Run(() =>
                {
                    using (CastorContext castorContext = new CastorContext())
                    {
                        castorContext.Backup();
                    }
                });

                // 4. Миграции
                _splashWindow.UpdateProgress("Миграции базы данных...", 60);
                await Task.Run(() =>
                {
                    try
                    {
                        using (CastorContext castorContext = new CastorContext())
                        {
                            castorContext.Database.Migrate();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError("Migrations", ex);
                        throw new Exception($"Ошибка обновления базы: {ex.Message}", ex);
                    }
                });

                // 5. Синхронизация базы (запускаем асинхронно, не дожидаясь завершения)
                _splashWindow.UpdateProgress("Синхронизация базы данных...", 80);

                // Запускаем синхронизацию в фоне, но не ждем ее завершения
                await Task.Run(() =>
                {
                    try
                    {
                        new Synchronization().LoadExistsFromMedis();
                    }
                    catch (Exception ex)
                    {
                        LogError("Synchronization", ex);
                        // Можно показать уведомление, но не прерывать запуск
                        Dispatcher.InvokeAsync(() =>
                        {
                            MessageBox.Show($"Ошибка синхронизации: {ex.Message}",
                                          "Предупреждение",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Warning);
                        });
                    }
                });

                // 6. Загружаем сохранённую тему
                _splashWindow.UpdateProgress("Загрузка темы...", 90);
                await Dispatcher.InvokeAsync(() =>
                {
                    ThemeManager.LoadSavedTheme();
                });

                // 7. Открываем главное окно
                _splashWindow.UpdateProgress("Запуск главного окна...", 100);

                await Dispatcher.InvokeAsync(() =>
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    // Закрываем Splash
                    _splashWindow.Close();
                });
            }
            catch (Exception ex)
            {
                // Обработка критических ошибок
                await Dispatcher.InvokeAsync(() =>
                {
                    LogError("Initialization", ex);
                    MessageBox.Show($"Критическая ошибка при инициализации:\n{ex.Message}",
                                  "Ошибка",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);

                    _splashWindow.Close();
                    Application.Current.Shutdown();
                });
            }
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
