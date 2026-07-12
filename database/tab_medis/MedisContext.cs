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

        public static bool IsMedisonnectionEnable =>
            PingHost();
             
        /// <summary>
        /// Constructor. Checks database to exsists, and creates it if not
        /// </summary>
        public MedisContext()
        {
            // EnsureCreated - функция проверки существования базы и ее создания в случае отрицательной проверки
            // для подключения к областной базе не нужна

            // Сначала пингуется хост, задержка в настройках, по умолчанию 500 мс, если пинга нет выбрасывается Exeption
            //PingHost();
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
                optionsBuilder.UseNpgsql(Decrypt(Settings.Default.postgreeConnection));
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
            var address = Regex.Match(Decrypt(Settings.Default.postgreeConnection),
                        @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").Value;

            using Ping pinger = new Ping();
            PingReply status = pinger.Send(address, Settings.Default.HostPingLatency);
            return status.Status == IPStatus.Success;
        }

        

        public static string Decrypt(string cipherText, string password = null)
        {
            var cipher = Convert.FromBase64String(cipherText);
            var pwd = !string.IsNullOrEmpty(password) ? Encoding.Default.GetBytes(password) : Array.Empty<byte>();
            var data = ProtectedData.Unprotect(cipher, pwd, DataProtectionScope.CurrentUser);
            return Encoding.Default.GetString(data);
        }
    }
}
