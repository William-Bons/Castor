using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
                    //checking 
                    if(castor.Movebooks
                        .Where(m => m.Visitid==Movebook.Visitid).Count() > 0)
                    {
                        MessageBox.Show("История уже загружена в книгу движения","Ошибка", MessageBoxButton.OK,MessageBoxImage.Error);
                        return;
                    }

                    // saving
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
                        .Include(d => d.Visits.Where(v => !v.dat1.HasValue || (DateTime.Today.ToUniversalTime() - v.dat1).Value.Days < 8 ))
                        .ThenInclude(v => v.Doctor)
                        .Include(d => d.Visits.Where(v => !v.dat1.HasValue || (DateTime.Today.ToUniversalTime( )- v.dat1).Value.Days < 8 ))
                        .ThenInclude(v => v.Patient)
                        .ThenInclude(p => p.Diagnoses)
                        .ThenInclude(d => d.Diagnos)
                        .ToList();

                    var a = new SelectObjectFromEnumerable(depList.First().Visits, "Patient.birthdate", "Patient.fullname");
                    
                    a.Selected += (object sel) =>
                    {
                        Visit = (visit)sel;

                        using (MedisContext medisContext = new MedisContext())
                        {
                            var DS = medisContext.diagnos
                                .Where(d => d.keyid == Visit.Patient.CurrentDs.Diagnos.rootid)?.First();

                            Movebook.Dsin = DS.code;
                        }

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visit)));
                        Movebook.Ordered = 0;
                        Movebook.Card_Id = Visit.num;
                        Movebook.Patientid = Visit?.Patient?.num;
                        Movebook.Fio = Visit?.Patient?.fullname;
                        Movebook.Birthdate = DateOnly.FromDateTime(Visit.Patient.birthdate.Value);
                        Movebook.Datein = DateOnly.FromDateTime(Visit.dat.Value);
                        Movebook.Visitid = Visit?.keyid;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Movebook)));

                    };

                    
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

        private void SelectDiagnosFromList(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = new SelectObjectFromEnumerable(Visit.Patient.Diagnoses.TakeLast(10), "dat","Diagnos.code","Diagnos.text");
                a.Selected += (sel) =>
                {
                    using (MedisContext medisContext = new MedisContext())
                    {
                        var DS = medisContext.diagnos
                            .Where(d => d.keyid == (sel as patdiag).Diagnos.rootid)?.First();

                        Movebook.Dsin = DS.code;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Movebook)));
                    }
                };
            }
            catch { }
        }
    }
}
