using Castor.database.tables;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;

namespace Castor.database.tab_medis
{
    public class MedisContext : DbContext
    {
        public enum ContextVariant { SQLITE, SQLSERVER, POSTGREE };
        public string[] VariantNames = {"Connected SQLITE Database", "Connected SQLSERVER Database", "Connected POSTGREE Database" };
        private ContextVariant _contextVariant;

        #region TALBES
        /// <summary>
        /// Tables in database
        /// </summary>
        public DbSet<patserv> patserv => Set<patserv>();
        public DbSet<patient> patient => Set<patient>();
        public DbSet<visit> visit => Set<visit>();
        public DbSet<dep> dep => Set<dep>();
        #endregion

        /// <summary>
        /// Constructor. Checks database to exsists, and creates it if not
        /// </summary>
        public MedisContext()
        {
            _contextVariant = ContextVariant.POSTGREE;
            //Database.EnsureCreated();
        }

        /// <summary>
        /// need for console output
        /// </summary>
        public string Variant => VariantNames[(int)_contextVariant];


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
                case ContextVariant.POSTGREE:
                    optionsBuilder.UseNpgsql(Properties.Settings.Default.postgreeConnection);
                    break;
                default:
                    throw new ArgumentException("Propertie `contextValiant` not set correctly");
            }
            ;
        }
    }
}
