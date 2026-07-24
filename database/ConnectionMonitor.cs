using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Castor.database
{
    /// <summary>
    /// Интерфейс для мониторинга соединений с БД
    /// </summary>
    public interface IConnectionMonitor : IDisposable
    {
        void RegisterContext(DbContext context);
        void UnregisterContext(DbContext context);
        IReadOnlyList<DbContext> GetActiveContexts();
        void CloseAllConnections();
        bool HasActiveConnections { get; }
        int ActiveConnectionsCount { get; }
    }

    /// <summary>
    /// Монитор открытых соединений с базами данных
    /// </summary>
    public class ConnectionMonitor : IConnectionMonitor
    {
        private readonly ConcurrentBag<DbContext> _activeContexts = new ConcurrentBag<DbContext>();
        private readonly object _lockObject = new object();
        private bool _disposed = false;

        /// <summary>
        /// Проверяет наличие активных соединений
        /// </summary>
        public bool HasActiveConnections
        {
            get
            {
                lock (_lockObject)
                {
                    return !_activeContexts.IsEmpty;
                }
            }
        }

        /// <summary>
        /// Количество активных соединений
        /// </summary>
        public int ActiveConnectionsCount
        {
            get
            {
                lock (_lockObject)
                {
                    return _activeContexts.Count;
                }
            }
        }

        /// <summary>
        /// Регистрирует контекст БД в мониторе
        /// </summary>
        /// <param name="context">Контекст для мониторинга</param>
        public void RegisterContext(DbContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (_disposed) throw new ObjectDisposedException(nameof(ConnectionMonitor));

            lock (_lockObject)
            {
                _activeContexts.Add(context);

                // Логируем регистрацию
                System.Diagnostics.Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] 🔌 Контекст ЗАРЕГИСТРИРОВАН: {context.GetType().Name}. " +
                    $"Активных соединений: {_activeContexts.Count}");
            }
        }

        /// <summary>
        /// Удаляет контекст из мониторинга
        /// </summary>
        /// <param name="context">Контекст для удаления</param>
        public void UnregisterContext(DbContext context)
        {
            if (context == null) return;
            if (_disposed) return;

            lock (_lockObject)
            {
                // Для ConcurrentBag нужно удалять через пересоздание коллекции
                var contexts = _activeContexts.ToList();
                bool removed = contexts.Remove(context);

                if (removed)
                {
                    // Очищаем текущую коллекцию и добавляем оставшиеся
                    while (!_activeContexts.IsEmpty)
                    {
                        _activeContexts.TryTake(out _);
                    }

                    foreach (var ctx in contexts)
                    {
                        _activeContexts.Add(ctx);
                    }

                    // Логируем удаление
                    System.Diagnostics.Debug.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss.fff}] 🔓 Контекст УДАЛЕН: {context.GetType().Name}. " +
                        $"Активных соединений: {_activeContexts.Count}");
                }
            }
        }

        /// <summary>
        /// Получает список активных контекстов
        /// </summary>
        /// <returns>Список активных контекстов</returns>
        public IReadOnlyList<DbContext> GetActiveContexts()
        {
            lock (_lockObject)
            {
                return _activeContexts.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Закрывает все активные соединения
        /// </summary>
        public void CloseAllConnections()
        {
            lock (_lockObject)
            {
                var contexts = _activeContexts.ToList();
                int count = contexts.Count;

                // Логируем начало закрытия
                System.Diagnostics.Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] 🚪 НАЧАЛО ЗАКРЫТИЯ всех соединений. Активных: {count}");

                foreach (var context in contexts)
                {
                    try
                    {
                        if (context.Database.CanConnect())
                        {
                            context.Database.CloseConnection();
                            context.Dispose();
                            System.Diagnostics.Debug.WriteLine(
                                $"[{DateTime.Now:HH:mm:ss.fff}]    Закрыт: {context.GetType().Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[{DateTime.Now:HH:mm:ss.fff}] ❌ Ошибка при закрытии {context.GetType().Name}: {ex.Message}");
                    }
                }

                // Очищаем коллекцию
                while (!_activeContexts.IsEmpty)
                {
                    _activeContexts.TryTake(out _);
                }

                // Логируем завершение
                System.Diagnostics.Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] ✅ ВСЕ СОЕДИНЕНИЯ ЗАКРЫТЫ. Осталось: {_activeContexts.Count}");
            }
        }

        /// <summary>
        /// Освобождает ресурсы монитора
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            lock (_lockObject)
            {
                CloseAllConnections();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Статический доступ к монитору соединений
    /// </summary>
    public static class ConnectionMonitorManager
    {
        private static readonly Lazy<IConnectionMonitor> _lazyInstance =
            new Lazy<IConnectionMonitor>(() => new ConnectionMonitor(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static IConnectionMonitor Instance => _lazyInstance.Value;

        /// <summary>
        /// Безопасное закрытие всех соединений
        /// </summary>
        public static void SafeCloseAllConnections()
        {
            try
            {
                var count = Instance.ActiveConnectionsCount;
                System.Diagnostics.Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] 🛑 Запрос на закрытие соединений. Активных: {count}");

                Instance.CloseAllConnections();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] ❌ Ошибка при закрытии соединений: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить информацию о состоянии соединений для логирования
        /// </summary>
        public static string GetConnectionStatus()
        {
            var count = Instance.ActiveConnectionsCount;
            var contexts = Instance.GetActiveContexts();

            if (count == 0)
                return "Нет активных соединений";

            var contextNames = string.Join(", ", contexts.Select(c => c.GetType().Name));
            return $"Активных соединений: {count} ({contextNames})";
        }

        /// <summary>
        /// Вывести статус соединений в лог и Debug
        /// </summary>
        public static void LogConnectionStatus(string prefix = "")
        {
            var status = GetConnectionStatus();
            var message = string.IsNullOrEmpty(prefix) ? status : $"{prefix} {status}";

            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] 📊 {message}");
        }
    }
}