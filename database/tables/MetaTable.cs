using System.Reflection;
using Microsoft.EntityFrameworkCore;


namespace Castor.database.tables
{
    public class MetaTable 
    {
        public PropertyInfo[] getInfo()
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            return properties;
        }

        public static async Task<bool> TableUsersExistsAsync(CastorContext context)
        {
            // ПРОВЕРКА ДЛЯ SQLITE (упрощенная, без параметров)
            const string sql = @"
            SELECT COUNT(*) 
            FROM sqlite_master 
            WHERE type = 'table' 
              AND name = 'Users';"; // Проверь регистр: Users или users


            var count = await context.Database.ExecuteSqlRawAsync(sql);
            return count > 0;
        }
    }
}
