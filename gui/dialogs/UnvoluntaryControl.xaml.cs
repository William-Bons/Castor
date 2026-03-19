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
    /// Логика взаимодействия для UnvoluntaryControl.xaml
    /// </summary>
    public partial class UnvoluntaryControl : Window, INotifyPropertyChanged
    {
        public UnvoluntaryControl(object _unvl)
        {
            if(_unvl is Unvoluntary unvol)
            {
                UnlItem = unvol;
            }
            else if(_unvl is Movebook mb)
            {
                UnlItem = new Unvoluntary() { Start = mb.Datein.Value };
            }

            InitializeComponent();
            DataContext = this;
        }

        public Unvoluntary UnlItem { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Calculate(object sender, RoutedEventArgs e)
        {
            UnlItem.Nextvk = UnlItem.Nextvk.HasValue ?
                UnlItem.CalcNextVk(UnlItem.Nextvk.Value) :
                UnlItem.CalcNextVk(UnlItem.Start);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnlItem)));
        }

        private void CloseEln(object sender, RoutedEventArgs e)
        {
            UnlItem.End = DateOnly.FromDateTime(DateTime.Today);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnlItem)));
        }

        private void Write(object sender, RoutedEventArgs e)
        {
            try
            {
                using(CastorContext castor = new CastorContext())
                {
                    castor.Unvoluntaries.Update(UnlItem);
                    castor.SaveChanges();
                }
                DialogResult = true;
                Close();
            }
            catch { }
        }
    }
}
