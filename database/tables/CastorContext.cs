using Castor.database.tables;
using Castor.gui;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Runtime.CompilerServices;
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
        public DbSet<Reports>  Reports => Set<Reports>();
        #endregion

        /// <summary>
        /// Constructor. Checks database to exsists, and creates it if not
        /// </summary>
        public CastorContext()
        {
            _contextVariant = (ContextVariant)Properties.Settings.Default.contextValiant;
            if (!Database.CanConnect())
            {
                CreateNewDatabase();
            }
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
                    //todo: Check file .db exists !!!!
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

        private void CreateNewDatabase()
        {
            FormattableString sqlLine;
            using(FileStream file = new FileStream("sqlite/movebook_create.sql", FileMode.Open))
            {
                using(TextReader reader = new StreamReader(file))
                {
                    sqlLine = FormattableStringFactory.Create(reader.ReadToEnd());
                }
            }

            try
            {
                Database.EnsureCreated();
                //Database.ExecuteSql(sqlLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
                
        }

    }
}
