using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using static Castor.gui.common.IDialog;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для CreateNewUser.xaml
    /// </summary>
    public partial class CreateNew : Window, IDialog, INotifyPropertyChanged, IConsoleMessage
    {
        public event DialogOKHandler? DialogOK;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event ConsoleMessageHandler? ConsoleMessage;

        public CreateNew()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Movebook Movebook { get; private set; } = new Movebook();
        public visit Visit { get; private set; } = new visit();

        private void SaveInDb(object sender, RoutedEventArgs e)
        {
            if(!Validate())
            {
                MessageBox.Show("Не все необходимые данные установлены","ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (CastorContext castor = new CastorContext())
            {
                try
                {
                    castor.Movebooks.Update(Movebook);
                    castor.SaveChanges();
                }
                catch (Exception ex)
                {
                    ConsoleMessage?.Invoke(ex.Message);
                }
            }

            DialogOK?.Invoke();
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SelectPatientFromMedis(object sender, RoutedEventArgs e)
        {
            using (MedisContext medisContext = new MedisContext())
            {
                try
                {
                    ICollection<dep>? depList = medisContext.dep
                        .Where(d => d.keyid == Settings.Default.LastSelectedDep)
                        .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                        .ThenInclude(v => v.Doctor)
                        .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                        .ThenInclude(v => v.Patient)
                        .ThenInclude(p => p.Diagnoses)
                        .ToList();

                    Popup popup = new Popup();
                    var a = new SelectPatientInVisits(depList.First().Visits);
                    a.Selected += (object sel) =>
                    {
                        popup.IsOpen = false;
                        Visit = (visit)sel;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visit)));
                        Movebook.Ordered = 0;
                        Movebook.Card_Id = Visit.num;
                        Movebook.Patientid = Visit?.Patient?.num;
                        Movebook.Fio = Visit?.Patient?.fullname;
                        Movebook.Birthdate = DateOnly.FromDateTime(Visit.Patient.birthdate.Value);
                        Movebook.Datein = DateOnly.FromDateTime(Visit.dat.Value);
                        Movebook.Visitid = Visit?.keyid;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Movebook)));
                        DateInControl.SelectedDate = Visit?.dat;

                    };
                    popup.Child = a;
                    popup.StaysOpen = false;
                    popup.Placement = PlacementMode.MousePoint;
                    popup.IsOpen = true;
                }
                catch(Exception ex)
                {
                    ConsoleMessage?.Invoke(ex.Message);
                }
            }
        }

        private bool Validate()
        {
            return (Movebook.Card_Id > 0 && Movebook.Datein > DateOnly.MinValue && !string.IsNullOrWhiteSpace(Movebook.Dsin));
        }
    }
}
