using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            {
                using CastorContext castor = new CastorContext();
                PatientRecord = castor.Movebooks
                    .First(m => m.Patientid == Settings.Default.LastForcedPatientID);

                LoadForcelistForPatient();
            }

            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// вызывается из Thebook, загрузка списка принудок для пациента
        /// </summary>
        public Forcepage(Movebook movebook)
        {
            PatientRecord = movebook;
            Settings.Default.LastForcedPatientID = movebook.Patientid.HasValue ? movebook.Patientid.Value : 0;
            Settings.Default.Save();
            LoadForcelistForPatient();

            InitializeComponent();
            DataContext = this;
        }

        public IEnumerable<Forced> ForceList { get; set; } = new List<Forced>();
        public Movebook? PatientRecord { get; set; }

        private void LoadForcelistForPatient()
        {
            try
            {
                using CastorContext castor = new CastorContext();

                ForceList = castor.Forced
                    .Where(f => f.Patientid == PatientRecord.Patientid)
                    .Include(f => f.Movebook)
                    .OrderBy(f => f.Start)
                    .ToList();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceList)));
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
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
        /// Добавляет новое постановление суда через диалог
        /// </summary>
        private void AddNewCourtOrder(object sender, RoutedEventArgs e)
        {
            if (ForceList.Count() > 0)
                new ForceControl(ForceList).ShowDialog(); // добавление пост. к списку имеющихся
            else
                new ForceControl(PatientRecord).ShowDialog();// добвление первичного постановления т.к. список пуст

            LoadForcelistForPatient();
        }

        /// <summary>
        /// редактирование выделенного постановления
        /// </summary>
        private void EditSelectedForceLine(object sender, MouseButtonEventArgs e)
        {
            new ForceControl(ForcesDataGrid.SelectedItem).ShowDialog(); // редактирование выделенного постановления

            LoadForcelistForPatient();
        }
    }
}
