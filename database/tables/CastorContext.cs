using Castor.database.tables;
using Castor.gui.dialogs;
using Castor.Properties;
using EfSchemaCompare;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Castor.database
{
    public class CastorContext : DbContext
    {
        /// <summary>
        /// Контекст подключения к локальной базе данных Castor
        /// </summary>
        public enum ContextVariant { SQLITE, SQLSERVER, POSTGREE };
        private ContextVariant _contextVariant = (ContextVariant)Settings.Default.contextValiant;

        #region TALBES
        /// <summary>
        /// Tables in database
        /// </summary>
        public DbSet<Movebook> Movebooks => Set<Movebook>();
        public DbSet<Reports> Reports => Set<Reports>();
        public DbSet<Forced> Forced => Set<Forced>();
        public DbSet<Fss> Fss => Set<Fss>();
        public DbSet<Unvoluntary> Unvoluntaries => Set<Unvoluntary>();
        public DbSet<Commity> Commity => Set<Commity>();
        public DbSet<Runs> Runs => Set<Runs>();
        public DbSet<User> Users => Set<User>();

        #endregion

        private bool _isDisposed = false;

        /// <summary>
        /// Constructor. Checks database to exsists, and creates it if not
        /// </summary>
        public CastorContext()
        {
            try
            {
                if (!Database.CanConnect())
                {
                    Database.EnsureCreated();
                }

                // Регистрируем контекст в мониторе
                ConnectionMonitorManager.Instance.RegisterContext(this);
            }
            catch (Exception ex)
            {
                new SelectUser().ShowDialog();
            }
        }

        public void Backup()
        {
            // создает ДБ backup in User Home directory
            try
            {
                // Access the underlying connection from your DbContext
                var connection = (SqliteConnection)Database.GetDbConnection();

                // Create a connection to the destination backup file
                using (var backupConnection = new SqliteConnection($"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\{Settings.Default.LastSelectedDepId}_backup.db"))
                {
                    connection.Open();
                    backupConnection.Open();

                    // Perform the backup
                    connection.BackupDatabase(backupConnection);
                }
            }
            catch { }
        }

        /// <summary>
        /// Real connecting to database, according Parameter contextVariant
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <exception cref="ArgumentException"></exception>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_contextVariant)
            {
                case ContextVariant.SQLITE:
                    optionsBuilder.UseSqlite($"Data Source={Settings.Default.sqliteConnection}");
                    break;
                case ContextVariant.SQLSERVER:
                    optionsBuilder.UseSqlServer(Settings.Default.sqlserverConnection);
                    break;
                default:
                    throw new ArgumentException("Property `contextValiant` not set correctly");
            }
        }

        public override void Dispose()
        {
            if (!_isDisposed)
            {
                // >>> ДОБАВЛЕНО: Удаляем контекст из монитора
                ConnectionMonitorManager.Instance.UnregisterContext(this);
                _isDisposed = true;

                System.Diagnostics.Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] 🔓 CastorContext УДАЛЕН из монитора");
            }

            base.Dispose();
        }
    }
}