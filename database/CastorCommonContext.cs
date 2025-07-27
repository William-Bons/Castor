using Castor.database.tables;
using Castor.gui;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;

namespace Castor.database
{
    public class CastorCommonContext : DbContext
    {
        public enum ContextVariant { SQLITE, SQLSERVER, POSTGREE };
        public string[] VariantNames = {"Connected SQLITE Database", "Connected SQLSERVER Database", "Connected POSTGREE Database" };
        private ContextVariant _contextVariant;

        #region TALBES
        /// <summary>
        /// Tables in database
        /// </summary>
        public DbSet<Planning> Plannings => Set<Planning>();
        public DbSet<DictPlannings> DictPlannings => Set<DictPlannings>();
        #endregion

        /// <summary>
        /// Constructor. Checks database to exsists, and creates it if not
        /// </summary>
        private CastorCommonContext()
        {
            _contextVariant = (ContextVariant)Properties.Settings.Default.contextValiant;
            if(!Database.CanConnect())
            {
                Database.EnsureCreated();

                DictPlannings.Add(new DictPlannings() { docdepid = 0, description = "ЭЛН", isprivate = true, period = 15 });
                DictPlannings.Add(new DictPlannings() { docdepid = 0, description = "НГ", isprivate = true, period = 30 });
                DictPlannings.Add(new DictPlannings() { docdepid = 0, description = "НГ СУД", isprivate = true, period = 180 });

                SaveChanges();
            }
            
        }

        public CastorCommonContext(ContextVariant variant)
        {
            _contextVariant = variant;

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
                    optionsBuilder.UseNpgsql("Host=172.23.1.220;Port=5433;Database=med;Username=SOLUTION_MED;Password=elsoft");
                    break;
                default:
                    throw new ArgumentException("Propertie `contextValiant` not set correctly");
            }
            ;
        }

        public static CastorCommonContext Get()
        {
            return new CastorCommonContext();
        }

        
    }
}
