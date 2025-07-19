using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
