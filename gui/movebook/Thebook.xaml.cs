using Castor.database;
using Castor.database.tables;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для Thebook.xaml
    /// </summary>
    public partial class Thebook : Page, INotifyPropertyChanged
    {
        private CastorContext context;
        private bool need_save = false;

        public Thebook()
        {
            InitializeComponent();
            DataContext = this;
            Task.Run(() => Load());
        }

        public ICollection<Movebook> LoadedData { get; private set; }
        public Visibility SaveButtonVisible => need_save ? Visibility.Visible : Visibility.Collapsed;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void PatientsTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            need_save = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonVisible)));
        }

        private async Task Load()
        {
            if (context != null) context.Dispose();

            context = new CastorContext();
            LoadedData = context.Movebooks.ToList();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadedData)));

        }

        private void NeedRefreshTable(object sender, System.Windows.RoutedEventArgs e)
        {
            need_save = false;
            context.SaveChanges(true);
            Load();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonVisible)));
        }

        private void PatientsTable_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if( ((DataGrid)sender).SelectedItem is Movebook mvb)
            {
                Disorder disorder = new Disorder(mvb);
                disorder.ShowDialog();
            }
        }
    }
}
