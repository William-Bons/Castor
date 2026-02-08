using Castor.database.tab_medis;
using Castor.database.tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Castor.test
{
    /// <summary>
    /// class to show ICollection content 
    /// </summary>
    public class TablePage
    {
        private Window host;

        public TablePage(string UserControlClass)
        {
            host = new Window() { Width = 400, Height = 600 };
            object NewClass = Activator.CreateInstance(Type.GetType(UserControlClass));
            host.Content = NewClass;
            host.Show();
        }

        

    }
}
