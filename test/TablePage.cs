using Castor.database.tab_medis;
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
        private ICollection<patdiag> rows;

        public TablePage(ICollection<patdiag> collection)
        {
            rows = collection;
            if (rows != null && rows.Count > 0)
            {
                CreateView();
            }
        }

        private void CreateView()
        {
            host = new Window() { Width = 400, Height = 600 };
            DataGrid dataGrid = new DataGrid();
            host.Content = dataGrid;

            dataGrid.ItemsSource = rows;
            host.Show();
        }
    }
}
