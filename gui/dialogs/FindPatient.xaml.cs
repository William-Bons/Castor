using Castor.database.tab_medis;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для FindPatient.xaml
    /// </summary>
    public partial class FindPatient : Window, INotifyPropertyChanged
    {
        public FindPatient()
        {
            InitializeComponent();
            new MoveFocusHelper(ObservePanel, [Key.Enter, Key.Return], TryToSearch, null);
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void TryToSearch(object src)
        {
            MainWindow.Wait(true);
            try
            {
                using MedisContext medis = new MedisContext();
                Patients = medis.patient
                    .Where(p =>
                        p.lastname.Contains(Patientdata.Family.ToLower())
                        && p.firstname.Contains(Patientdata.Firstname.ToLower())
                        && p.secondname.Contains(Patientdata.Lastname.ToLower())
                    ).Take(20)
                    .ToList();
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }

            MainWindow.Wait();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Patients)));
        }

        public patient Patient { get; set; }
        public Patientdata Patientdata { get; set; } = new Patientdata();
        public IEnumerable<patient> Patients { get; set; }
        public IEnumerable<visit> Visits { get; set; }
        public visit Visit { get; set; }
        public bool OnlySelectedDep { get; set; } = true;

        private void Exit(object sender, RoutedEventArgs e)
        {
            Patient = null;
            DialogResult = false;
            Close();
        }

        private void SelectPatient(object sender, RoutedEventArgs e)
        {
            if (Visit == null)
            {
                if (Patient == null)
                    Patient = Patients?.First();

                Visit = Patient?.Visits?.First();
            }
#if DEBUG
            TablePage.ShowPopup(Visit);
#endif
            DialogResult = true;
            Close();
        }

        private void Patients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Patient is not null)
            {
                try
                {
                    using MedisContext medis = new MedisContext();
                    Visits = medis.visit
                        .Where(v => v.patientid == Patient.keyid)
                        .Include(v => v.Dep)
                        .ThenInclude(r => r.Root).ThenInclude(t => t.Root)
                        .Include(v => v.Patient)
                        .ThenInclude(p => p.Diagnoses.Where(g => g.Diagnos.code.StartsWith("F")))
                        .ThenInclude(d => d.Diagnos)
                        .ToList();

                    if (OnlySelectedDep)
                    {
                        Visits = Visits.Where(v => v.depid == Settings.Default.LastSelectedDepId).ToList();
                    }
                }
                catch (Exception ex)
                {
                    Message.ShowPopup(ex.Message);
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visits)));
            }
        }

    }

    public class Patientdata
    {
        public string Firstname { get; set; } = string.Empty;
        public string Family { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string SNILS { get; set; } = string.Empty;
        public virtual string Fullname => $"{Family} {Firstname} {Lastname}";
    }
}
