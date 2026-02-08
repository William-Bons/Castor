using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.Properties;
using Castor.test;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using static Castor.gui.common.IDialog;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для CreateNewUser.xaml
    /// </summary>
    public partial class CreateNew : Window, IDialog, INotifyPropertyChanged
    {
        public event DialogOKHandler DialogOK;
        public event PropertyChangedEventHandler? PropertyChanged;

        private CastorContext _castorContext;
        public CreateNew()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Movebook Movebook { get; private set; } = new Movebook();
        public visit Visit {  get; private set; }

        private void SaveInDb(object sender, RoutedEventArgs e)
        {
            using(CastorContext castor=new CastorContext())
            {
                FormattableString fstr = $@"INSERT INTO Movebooks (card_id, patientid, fio, birthdate, datein, dateout, ordered, dsin, city,first,second,early,unvoluntary,date_lastout) VALUES ({Movebook.Card_Id}, {Movebook.Patientid},{Movebook.Fio},{Movebook.Birthdate},{Movebook.Datein},{Movebook.Dateout},{Movebook.Ordered},{Movebook.Dsin},{Movebook.City},{Movebook.First},{Movebook.Second},{Movebook.Early},{Movebook.Unvoluntary},{Movebook.Date_Lastout})";

                castor.Database.ExecuteSql(fstr);
                //castor.Movebooks.Add(Movebook);
                castor.SaveChanges();
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
            if(MedisContext.PingHost() == System.Net.NetworkInformation.IPStatus.Success)
                using (MedisContext medisContext = new MedisContext())
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
                        Movebook.Card_Id = (int)Visit.num;
                        Movebook.Patientid = (int)Visit.Patient.num;
                        Movebook.Fio = Visit.Patient.fullname;
                        Movebook.Birthdate = DateOnly.FromDateTime(Visit.Patient.birthdate.Value);
                        Movebook.Datein = DateOnly.FromDateTime(Visit.dat.Value);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Movebook)));
                        DateInControl.SelectedDate = Visit.dat;

                    };
                    popup.Child = a;
                    popup.StaysOpen = false;
                    popup.Placement = PlacementMode.MousePoint;
                    popup.IsOpen = true;
                }
        }
    }
}
