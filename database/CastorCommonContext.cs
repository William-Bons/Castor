using Castor.database.tables;
using Microsoft.EntityFrameworkCore;

namespace Castor.database
{
    public abstract class CastorCommonContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public CastorCommonContext() => Database.EnsureCreated();


        // This code must be realized in real Context classes 

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("Data Source=helloapp.db");
        //}
    }
}
