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
    /// Логика взаимодействия для ForceControl.xaml
    /// </summary>
    public partial class ForceControl : Window, INotifyPropertyChanged
    {
        public ForceControl(object _force)
        {
            if(_force is Forced forced)
            {
                ForcedItem = forced;
            }
            else if(_force is Movebook mb)
            {
                ForcedItem = new Forced() { Start = mb.Datein.Value };
            }

            InitializeComponent();
            DataContext = this;
        }

        public Forced ForcedItem { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Calculate(object sender, RoutedEventArgs e)
        {
            ForcedItem.Nextvk = ForcedItem.Nextvk.HasValue ?
                ForcedItem.CalcNextVk(ForcedItem.Nextvk.Value) :
                ForcedItem.CalcNextVk(ForcedItem.Start);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForcedItem)));
        }

        private void CloseEln(object sender, RoutedEventArgs e)
        {
            ForcedItem.End = DateOnly.FromDateTime(DateTime.Today);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForcedItem)));
        }

        private void Write(object sender, RoutedEventArgs e)
        {
            try
            {
                using(CastorContext castor = new CastorContext())
                {
                    castor.Forced.Update(ForcedItem);
                    castor.SaveChanges();
                }
                DialogResult = true;
                Close();
            }
            catch { }
        }
    }
}
