using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для ImportMedisPeriod.xaml
    /// </summary>
    public partial class ImportMedisPeriod : Window, INotifyPropertyChanged
    {
        public ImportMedisPeriod()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ObservableCollection<Movebook> DataRowSource { get; private set; }
        public DatePeriod DatePeriod { get; set; } = new DatePeriod() { End = DateTime.Now, Start = DateTime.Now.AddDays(-30) };

        public event PropertyChangedEventHandler? PropertyChanged;

        private async void LoadRecords(object sender, RoutedEventArgs e)
        {
            DatePeriod = new DatePeriod() { Start=DP0.SelectedDate.Value, End=DP1.SelectedDate.Value };
            
            DataRowSource = new ObservableCollection<Movebook>();

            Func<Task> Foo = async () =>
            {
                Cursor = Cursors.Wait;
                try
                {
                    using (MedisContext medis = new MedisContext())
                    {

                        ICollection<dep>? depList = medis.dep
                                .Where(d => d.keyid == Settings.Default.LastSelectedDep)
                                .Include(d => d.Visits.Where(v => (v.dat >= DatePeriod.Start.ToUniversalTime() && v.dat <= DatePeriod.End.ToUniversalTime())
                                        || (v.dat1 >= DatePeriod.Start.ToUniversalTime() && v.dat1 <= DatePeriod.End.ToUniversalTime())))
                                .ThenInclude(v => v.Patient)
                                .ThenInclude(p => p.Diagnoses)
                                .ToList();

                        ICollection<visit>? visits = depList?.First()?.Visits?.ToList() ?? new List<visit>();

                        foreach (var vis in visits)
                        {
                            Movebook movebook = new Movebook();
                            movebook.Fio = vis.Patient?.fullname ?? string.Empty;
                            movebook.Datein = vis.dat.HasValue ? DateOnly.FromDateTime(vis.dat.Value) : null;
                            movebook.Ordered = 0;
                            movebook.Card_Id = vis.num;
                            movebook.Patientid = vis?.Patient?.num;
                            movebook.Birthdate = DateOnly.FromDateTime(vis.Patient.birthdate.Value);
                            movebook.Visitid = vis?.keyid;

                            diagnos? dsIn = medis.diagnos
                                .Where(d => d.keyid == vis.Patient.CurrentDs.diagid)?.First();
                            movebook.Dsin = dsIn?.code;

                            if (vis.dat1.HasValue)
                            {
                                movebook.Dateout = vis.dat1.HasValue ? DateOnly.FromDateTime(vis.dat1.Value) : null;
                                diagnos? dsOut = medis.diagnos
                                    .Where(d => d.keyid == vis.Patient.CurrentDs.diagid)?.First();
                                movebook.Dsout = dsOut?.code;
                            }

                            DataRowSource.Add(movebook);
                        }

                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataRowSource)));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                Cursor = Cursors.Arrow;
            };
            await Foo();
        }

        private void ShowAllDiagnosis(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ImportDataGrid.SelectedItem is Movebook book)
                {

                    using (MedisContext medis = new MedisContext())
                    {
                        ICollection<patient> dss = medis.patient
                            .Where(p => p.num == book.Patientid)
                            .Include(p => p.Diagnoses)
                            .ToList();

                        DataGrid dataGrid = new DataGrid() { Language = XmlLanguage.GetLanguage("ru-RU") };
                        dataGrid.ItemsSource = dss.First().Diagnoses;
                        Window window = new Window();
                        window.Content = dataGrid;
                        window.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
