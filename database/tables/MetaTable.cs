using System.Reflection;

namespace Castor.database.tables
{
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
