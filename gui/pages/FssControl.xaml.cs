using Castor.database;
using Castor.database.tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Castor.gui.pages
{
    /// <summary>
    /// Логика взаимодействия для FssControl.xaml
    /// </summary>
    public partial class FssControl : UserControl, INotifyPropertyChanged
    {
        public FssControl()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += (a, b) =>
            {
                Task.Run(() => Calculate());
            };
        }

        public ICollection<Movebook> FssList { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async Task Calculate()
        {
            try
            {
                using (CastorContext castor = new CastorContext())
                {
                    FssList = castor.Movebooks.Where(x => x.Fss.HasValue).ToList();
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FssList)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
