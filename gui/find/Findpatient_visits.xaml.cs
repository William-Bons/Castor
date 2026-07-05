using Castor.gui.common;
using System.Windows.Controls;

namespace Castor.gui.find
{
    /// <summary>
    /// Логика взаимодействия для Findpatient_visits.xaml
    /// </summary>
    public partial class Findpatient_visits : DataGrid
    {
        public Findpatient_visits()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TablePage.ShowPopup(((DataGrid)sender).SelectedItem);
        }
    }
}
