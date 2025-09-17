using System.Reflection;

namespace Castor.database.tables
{
    [Obsolete]
    public abstract class MetaTable 
    {
        public PropertyInfo[] getInfo()
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            return properties;
        }
    }
}
