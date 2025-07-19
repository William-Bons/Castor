using Castor.database.tables;
using Microsoft.EntityFrameworkCore;

namespace Castor.database
{
    public class CastorCommonContext : DbContext
    {
        public enum ContextValiant { SQLITE, SQLSERVER, POSTGREE };
        public string[] VariantNames = {"Connected SQLITE Database", "Connected SQLSERVER Database", "Connected POSTGREE Database" };

        #region TALBES
        /// <summary>
        /// Tables in database
        /// </summary>
        public DbSet<User> Users => Set<User>();
        public DbSet<Person> Persons => Set<Person>();
        public DbSet<Medcard> MedCards => Set<Medcard>();
        public DbSet<Planning> Plannings => Set<Planning>();
        public DbSet<DictPlannings> DictPlannings => Set<DictPlannings>();
        #endregion
        public CastorCommonContext()
        {
            Database.EnsureCreated();
        }

        public string Variant => VariantNames[Properties.Settings.Default.contextValiant];

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch ((ContextValiant)Properties.Settings.Default.contextValiant)
            {
                case ContextValiant.SQLITE:
                    optionsBuilder.UseSqlite(Properties.Settings.Default.sqliteConnection);
                    break;
                case ContextValiant.SQLSERVER:
                    optionsBuilder.UseSqlServer(Properties.Settings.Default.sqlserverConnection);
                    break;
                case ContextValiant.POSTGREE:
                    optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=usersdb;Username=postgres;Password=здесь_указывается_пароль_от_postgres");
                    break;
                default:
                    throw new ArgumentException("Propertie `contextValiant` not set correctly");
            }
            
        }

        public static CastorCommonContext Get()
        {
            return new CastorCommonContext();
        }

        
    }
}
