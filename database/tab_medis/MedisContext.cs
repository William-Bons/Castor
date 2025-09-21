using Castor.database.tables;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Windows;

namespace Castor.database.tab_medis
{
    /// <summary>
    /// Класс контекста данных базы данных Медис
    /// осуществляет подключение к базе данных и дальнейшеее взаимодействие с ней
    /// подключение - Postgree
    /// </summary>
    public class MedisContext : DbContext
    {

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
            // EnsureCreated - функция проверки существования базы и ее создания в случае отрицательной проверки
            // для подключения к областной базе не нужна
            // Database.EnsureCreated(); -- uncomment if you want create new database
        }

        /// <summary>
        /// need for console output
        /// </summary>
        public string Variant => $"POSSTGREE: {Database.GetDbConnection().DataSource} @ {Database.GetDbConnection().Database}";


        /// <summary>
        /// Real connecting to database, according Parameter contextVariant
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <exception cref="ArgumentException"></exception>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseNpgsql(Properties.Settings.Default.postgreeConnection);
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Rised exception:\n{ex.Message}", ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static IPStatus PingHost(string nameOrAddress= "172.23.1.220")
        {
            try
            {
                using (Ping pinger = new Ping())
                {
                    PingReply reply = pinger.Send(nameOrAddress,1000);
                    return reply.Status;
                }
            }
            catch (PingException)
            {
                return IPStatus.Unknown;
            }
        }
    }
}
