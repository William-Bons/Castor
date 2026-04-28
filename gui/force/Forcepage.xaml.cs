using Castor.database;
using Castor.database.tab_medis;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Castor.gui.force
{
    /// <summary>
    /// Логика взаимодействия для Forcepage.xaml
    /// </summary>
    public partial class Forcepage : Page, INotifyPropertyChanged
    {

        public Forcepage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Forcepage(Movebook movebook)
            : this()
        {
            Loaded += (a, b) =>
            {
                using(CastorContext castor = new CastorContext())
                {
                    Forceds = castor.Forced
                        .Where(f => f.Patientid == movebook.Patientid)
                        .ToList();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Forceds)));
                }

                if(Forceds.Count()<1)
                    new ForceControl(movebook).ShowDialog();
            };
        }

        public IEnumerable<Forced> Forceds { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OpenHideContextMenu(object sender, RoutedEventArgs e)
        {
            ((Button)sender).ContextMenu.IsOpen = true;
        }

        /// <summary>
        /// Добавляет постановление суда через диалог
        /// </summary>
        private void AddNewCourtOrder(object sender, RoutedEventArgs e)
        {
            new ForceControl(ForcesDataGrid.SelectedItem).ShowDialog();
        }
    }
}
