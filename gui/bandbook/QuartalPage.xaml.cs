using Castor.database.tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Castor.gui.bandbook
{
    public delegate void BandbookItemSelectedHandler(Bandbook bandbook);

    /// <summary>
    /// Логика взаимодействия для QuartalPage.xaml
    /// </summary>
    public partial class QuartalPage : UserControl
    {
        public QuartalPage(ICollection<Bandbook> quartalData)
        {
            QuartalData = quartalData;
            InitializeComponent();
            DataContext = this;
        }

        public IEnumerable<Bandbook> QuartalData { get; set; }
        public event BandbookItemSelectedHandler BandbookItemSelected;

        private void QuartalDataGrid_Selected(object sender, RoutedEventArgs e)
        {
            BandbookItemSelected.Invoke((Bandbook)QuartalDataGrid.SelectedItem);
        }

    }
}
