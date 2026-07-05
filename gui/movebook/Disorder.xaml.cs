using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using static Castor.gui.common.IDialog;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для Disorder.xaml
    /// </summary>
    public partial class Disorder : Window, IDialog
    {
        public event DialogOKHandler? DialogOK;
        public Disorder(Movebook movebook)
        {
            Movebook = movebook;
            InitializeComponent();
            

            //PRELOAD
            try
            {
                using (MedisContext medis = new MedisContext())
                {
                    visit visit = medis.visit
                        .Where(v => v.keyid == movebook.Visitid)
                        .Include(v => v.Patient)
                        .ThenInclude(v => v.Diagnoses).First();

                    if (visit != null && visit.Patient.CurrentDs.diagid != null && visit.dat1.HasValue)
                    {
                        var DS = medis.diagnos
                                .Where(d => d.keyid == visit.Patient.CurrentDs.diagid)?.First();

                        var RD = medis.diagnos
                            .Where(d => d.keyid == DS.rootid)?.First();

                        Movebook.Dateout = DateOnly.FromDateTime(visit.dat1.Value);
                        Movebook.Dsout = RD.code;
                    }
                    else
                    {
                        Movebook.Dateout = DateOnly.FromDateTime(DateTime.Today);
                        Movebook.Dsout = Movebook.Dsin;
                    }
                }

                Movebook.Outto = 0;
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }

            DataContext = this;
        }

        public Movebook Movebook { get; set; }

        private void SaveInDb(object sender, RoutedEventArgs e)
        {
            if (!Validate())
            {
                MessageBox.Show("Не все необходимые данные введены","ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (CastorContext castor = new CastorContext())
            {
                //PatientRecord.Dateout = DateOnly.FromDateTime(DateInControl.SelectedDate.Value);
                //FormattableString fstr = $@"UPDATE Movebooks set dsout={PatientRecord.Dsout}, dateout={PatientRecord.Dateout}, city={PatientRecord.City}, early={PatientRecord.Early}, unvoluntary={PatientRecord.Unvoluntary}, first={PatientRecord.First}, second={PatientRecord.Second}, closed={PatientRecord.Closed}, outto={PatientRecord.Outto}, deceased={PatientRecord.Deceased}
                //  where Id={PatientRecord.Id}";
                //castor.Database.ExecuteSql(fstr);
                castor.Update(Movebook);
                castor.SaveChanges();
            }
            DialogOK?.Invoke();
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Movebook.Dsout) && Movebook.Outto.HasValue && DateInControl.SelectedDate.HasValue;
        }
    }
}
