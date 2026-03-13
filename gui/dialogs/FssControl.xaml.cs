using Castor.database;
using Castor.database.tables;
using System.ComponentModel;
using System.Windows;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для FssWidget.xaml
    /// </summary>
    public partial class FssControl : Window, INotifyPropertyChanged
    {
        public FssControl(object _fss)
        {
            if (_fss is Movebook mb)
                FssItem = new Fss() { Start = mb.Datein.Value };
            else
                FssItem = (Fss?)_fss;

            InitializeComponent();
            DataContext = this;
        }

        public Fss FssItem { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Calculate(object sender, RoutedEventArgs e)
        {
            FssItem.Nextvk = FssItem.Nextvk.HasValue ?
                FssItem.CalcNextVk(FssItem.Nextvk.Value) :
                FssItem.CalcNextVk(FssItem.Start).AddDays(-1);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FssItem)));
        }

        private void CloseEln(object sender, RoutedEventArgs e)
        {
            FssItem.End = DateOnly.FromDateTime(DateTime.Today);
        }

        private void Write(object sender, RoutedEventArgs e)
        {
            using (CastorContext castor = new CastorContext())
            {
                castor.Update(FssItem);
                castor.SaveChanges();
                DialogResult = true;
                Close();
            }
        }
    }
}
