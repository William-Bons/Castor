using Castor.database.medis_delegat;
using Castor.database.tab_medis;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.find
{
    /// <summary>
    /// Логика взаимодействия для FindPatient.xaml
    /// </summary>
    public partial class FindPatient : Window, INotifyPropertyChanged
    {
        public FindPatient()
        {
            InitializeComponent();
            new MoveFocusHelper(ObservePanel, [Key.Enter, Key.Return], /*TryToSearch*/null, null);
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

       

        public patient Patient { get; set; }
        public visit Visit { get; set; }
        public dep Department {  get; set; }
        public Patientdata Patientdata { get; set; } = new Patientdata();
        
        
        public IEnumerable<patient> Patients { get; set; }
        public IEnumerable<visit> Visits { get; set; }
        public IEnumerable<dep> Deps { get; set; }
        
        public bool OnlySelectedDep { get; set; } = true;

        private void Exit(object sender, RoutedEventArgs e)
        {
            Patient = null;
            DialogResult = false;
            Close();
        }

        private void SelectPatient(object sender, RoutedEventArgs e)
        {
            MainWindow.Wait(true);
            Patients = new FindPatientsAndVisits().FindPatient(Patientdata);
            MainWindow.Wait();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Patients)));
            if(Patients.Count()>0)
                UploadDatagrid(Patients);
        }

        private void SelectVisits(object sender, RoutedEventArgs e)
        {
            MainWindow.Wait(true);
            Visits = new FindPatientsAndVisits().FindVisitsForPatient(Patient, null); // todo: Department need !!!
            MainWindow.Wait();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visits)));
            if (Visits.Count() > 0)
                UploadDatagrid(Visits);
        }

        private void UploadDatagrid(object _DataContext)
        {
            CentralGridPanel.Children.OfType<DataGrid>().ToList().ForEach(CentralGridPanel.Children.Remove);

            FrameworkElement? panel = new Grid();
            if(_DataContext is IEnumerable<patient>)
            {
                panel = new Findpatient_patients();
            }
            if(_DataContext is IEnumerable<visit>)
            {
                panel = new Findpatient_visits();
            }

            panel.DataContext = this;
            Grid.SetColumn(panel, 2);
            CentralGridPanel.Children.Add(panel);
        }
    }

    public class Patientdata
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Firstname { get; set; } = string.Empty;
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Family { get; set; } = string.Empty;
        /// <summary>
        /// Отчество
        /// </summary>
        public string Lastname { get; set; } = string.Empty;
        public string SNILS { get; set; } = string.Empty;
        public virtual string Fullname => $"{Family} {Firstname} {Lastname}";
    }
}
