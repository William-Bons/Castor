using Microsoft.EntityFrameworkCore;

namespace Castor.database
{
    public class CastorSqlServerContext : CastorCommonContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Properties.Settings.Default.sqlserverConnection);
        }
    }
}
