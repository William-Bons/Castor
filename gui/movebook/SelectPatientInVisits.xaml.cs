using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для SelectPatientInVisits.xaml
    /// </summary>
    public partial class SelectPatientInVisits : UserControl
    {
        public delegate void SelectPatientInVisitsEventHandler(object selected);
        public event SelectPatientInVisitsEventHandler Selected;
        public SelectPatientInVisits(object _RowSourceObject)
        {
            RowSourceObject = _RowSourceObject;
            InitializeComponent();
            DataContext = this;
        }

        public object RowSourceObject { get; private set; }
        public object SelectedVisit { get; private set; }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedVisit = ((DataGrid)sender).SelectedItem;
            Selected?.Invoke(SelectedVisit);
        }
    }
}
