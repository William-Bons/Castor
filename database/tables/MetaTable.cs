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

        public static bool TableUsersExistsAsync(CastorContext context)
        {
            // ПРОВЕРКА ДЛЯ SQLITE (упрощенная, без параметров)
            var row = context.Database
                .SqlQueryRaw<string>(@"PRAGMA table_info('Users')")
                .ToList();

            bool tableExists = row.Any();
            return tableExists;
        }
    }
}
