using Castor.database.tables;
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
        public string[] VariantNames = { "SQLITE", "SQLSERVER", "POSTGREE" };
        private ContextVariant _contextVariant;

        #region TALBES
        /// <summary>
        /// Tables in database
        /// </summary>
        public DbSet<Movebook> Movebooks => Set<Movebook>();
        public DbSet<Reports> Reports => Set<Reports>();
        public DbSet<Forced> Forced => Set<Forced>();
        public DbSet<Fss> Fss => Set<Fss>();
        public DbSet<Unvoluntary> Unvoluntaries => Set<Unvoluntary>();
        public DbSet<Bandbook> Bandbooks => Set<Bandbook>();
        #endregion

        /// <summary>
        /// Constructor. Checks database to exsists, and creates it if not
        /// </summary>
        public CastorContext()
        {
            _contextVariant = (ContextVariant)Settings.Default.contextValiant;
            if (!Database.CanConnect())
            {
                Database.EnsureCreated();
            }
        }

        /// <summary>
        /// need for console output
        /// </summary>
        public string Variant => $"{VariantNames[(int)_contextVariant]}: {Database.GetDbConnection().DataSource} @ {Database.GetDbConnection().Database}";
        public string Errors {  get; private set; }

        public bool DBHasErrors()
        {
            var comparer = new CompareEfSql();

            //ATTEMPT
            //This will compare EF Core model of the database with
            //the database that the context's connection points to
            var hasErrors = comparer.CompareEfWithDb(this);

            //VERIFY
            //The CompareEfWithDb method returns true if there were errors. 
            //The comparer.GetAllErrors property returns a string, with each error on a separate line
            if (hasErrors)
                Errors = comparer.GetAllErrors;
                //MessageBox.Show(comparer.GetAllErrors,"DB Errors", MessageBoxButton.OK, MessageBoxImage.Error);
            return hasErrors;
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
                    throw new ArgumentException("Propertie `contextValiant` not set correctly");
            }
            ;
        }

    }
}
