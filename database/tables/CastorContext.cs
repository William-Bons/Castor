using Castor.database.tables;
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
        public DbSet<Planning> Plannings => Set<Planning>();
        public DbSet<dictionary> DictPlannings => Set<dictionary>();
        public DbSet<Movebook> Movebooks => Set<Movebook>();
        #endregion

        /// <summary>
        /// Constructor. Checks database to exsists, and creates it if not
        /// </summary>
        private CastorContext()
        {
            _contextVariant = (ContextVariant)Properties.Settings.Default.contextValiant;
            if (!Database.CanConnect())
            {
                MessageBox.Show("SQLITE Database is not exist", "SQLITE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public CastorContext(ContextVariant variant)
        {
            _contextVariant = variant;

        }

        /// <summary>
        /// need for console output
        /// </summary>
        public string Variant => $"{VariantNames[(int)_contextVariant]}: {Database.GetDbConnection().DataSource} @ {Database.GetDbConnection().Database}";


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
                    optionsBuilder.UseSqlite(Properties.Settings.Default.sqliteConnection);
                    break;
                case ContextVariant.SQLSERVER:
                    optionsBuilder.UseSqlServer(Properties.Settings.Default.sqlserverConnection);
                    break;
                default:
                    throw new ArgumentException("Propertie `contextValiant` not set correctly");
            }
            ;
        }

        public static CastorContext Get()
        {
            return new CastorContext();
        }


    }
}
