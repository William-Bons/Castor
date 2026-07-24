using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Castor.database.tab_medis
{
    /// <summary>
    /// Класс контекста данных базы данных Медис
    /// осуществляет подключение к базе данных и дальнейшеее взаимодействие с ней
    /// подключение - Postgree
    /// Host=172.23.1.220;Port=5432;Database=med;Username=SOLUTION_MED;Password=elsoft;
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
        public DbSet<diagnos> diagnos => Set<diagnos>();
        public DbSet<patdiag> patdiag => Set<patdiag>();
        public DbSet<docdep> docdep => Set<docdep>();
        public DbSet<lu> lu => Set<lu>();
        #endregion

        public static bool IsMedisonnectionEnable => PingHost();

        private bool _isDisposed = false;

        /// <summary>
        /// Constructor. Checks database to exsists, and creates it if not
        /// </summary>
        public MedisContext()
        {
            // Регистрируем контекст в мониторе
            ConnectionMonitorManager.Instance.RegisterContext(this);

            System.Diagnostics.Debug.WriteLine(
            $"[{DateTime.Now:HH:mm:ss.fff}] 🔌 MedisContext ЗАРЕГИСТРИРОВАН");
        }

        /// <summary>
        /// Real connecting to database, according Parameter contextVariant
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <exception cref="ArgumentException"></exception>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseNpgsql(new EncryptionHelper().Decrypt(Settings.Default.postgreeConnection));
            }
            catch (CryptographicException f_ex)
            {
                new ConnectionDialog().ShowDialog();
            }
            catch (Exception ex)
            {
                Message.ShowPopup($"Rised exception:\n{ex.Message}");
            }
        }

        public static bool PingHost()
        {
            var address = Regex.Match(new EncryptionHelper().Decrypt(Settings.Default.postgreeConnection),
                        @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").Value;

            using Ping pinger = new Ping();
            PingReply status = pinger.Send(address, Settings.Default.HostPingLatency);
            return status.Status == IPStatus.Success;
        }

               
        public override void Dispose()
        {
            if (!_isDisposed)
            {
                // >>> ДОБАВЛЕНО: Удаляем контекст из монитора
                ConnectionMonitorManager.Instance.UnregisterContext(this);
                _isDisposed = true;

                System.Diagnostics.Debug.WriteLine(
                    $"[{DateTime.Now:HH:mm:ss.fff}] 🔓 MedisContext УДАЛЕН из монитора");
            }

            base.Dispose();
        }
    }
}