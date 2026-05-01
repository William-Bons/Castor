using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.Properties;
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
    /// Логика взаимодействия для Forcepage.xaml // принудки пациента
    /// </summary>
    public partial class Forcepage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Аавтоматическая загрузка списка принудок из номера сохраненного в параметрах
        /// </summary>
        public Forcepage()
        {
            if (Settings.Default.LastForcedPatientID > 0)
                LoadForcelistForPatient(Settings.Default.LastForcedPatientID);

            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// вызывается из Thebook, загрузка списка принудок для пациента
        /// </summary>
        public Forcepage(Movebook movebook)
        {
            Settings.Default.LastForcedPatientID = movebook.Patientid.HasValue ?  movebook.Patientid.Value : 0;
            Settings.Default.Save();
            LoadForcelistForPatient(Settings.Default.LastForcedPatientID);

            InitializeComponent();
            DataContext = this;
        }

        public IEnumerable<Forced> ForceList { get; set; } = new List<Forced>();

        

        private void LoadForcelistForPatient(long patientid)
        {
            using (CastorContext castor = new CastorContext())
            {
                ForceList = castor.Forced
                    .Where(f => f.Patientid == patientid)
                    .ToList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceList)));
            }
        }

        private Forced? GetRootForce()
        {
            return ForceList.Count() > 0 ?
                ForceList.MinBy(f => f.Start) :
                null;
        }

        private Forced? GetLastForce()
        {
            return ForceList.Count() > 0 ?
                ForceList.MaxBy(f => f.Start) : 
                null;
        }

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
