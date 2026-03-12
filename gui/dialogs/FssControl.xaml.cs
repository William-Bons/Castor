using Castor.database;
using Castor.database.tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для FssWidget.xaml
    /// </summary>
    public partial class FssControl : Window, INotifyPropertyChanged
    {
        public FssControl(Fss? _fss)
        {
            FssItem = _fss ?? new Fss() { Start = DateOnly.FromDateTime(DateTime.Today) };
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
            using(CastorContext castor=new CastorContext())
            {
                castor.Update(FssItem);
                castor.SaveChanges();
                Close();
            }
        }
    }
}
