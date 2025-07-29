using Castor.database.tables;
using Castor.gui;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;

namespace Castor.database
{
    public class CastorContext : DbContext
    {
        public enum ContextVariant { SQLITE, SQLSERVER, POSTGREE };
        public string[] VariantNames = {"Connected SQLITE Database", "Connected SQLSERVER Database", "Connected POSTGREE Database" };
        private ContextVariant _contextVariant;

        #region TALBES
        /// <summary>
        /// Tables in database
        /// </summary>
        public DbSet<planning> Plannings => Set<planning>();
        public DbSet<dictionary> DictPlannings => Set<dictionary>();
        #endregion

        /// <summary>
        /// Constructor. Checks database to exsists, and creates it if not
        /// </summary>
        private CastorContext()
        {
            _contextVariant = (ContextVariant)Properties.Settings.Default.contextValiant;
            if(!Database.CanConnect())
            {
                Database.EnsureCreated();

                DictPlannings.Add(new dictionary() { description = "ЭЛН", isprivate = true, period = 15 });
                DictPlannings.Add(new dictionary() { description = "НГ", isprivate = true, period = 30 });
                DictPlannings.Add(new dictionary() { description = "НГ СУД", isprivate = true, period = 180 });
                DictPlannings.Add(new dictionary() { description = "Хронизация", isprivate = true, period = 90 });
                DictPlannings.Add(new dictionary() { description = "Инвалидность", isprivate = true, period = 30 });
                DictPlannings.Add(new dictionary() { description = "ПНИ", isprivate = true, period = 30 });

                SaveChanges();
            }
            
        }

        public CastorContext(ContextVariant variant)
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
