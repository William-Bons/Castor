using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для ImportMedisPeriod.xaml
    /// </summary>
    public partial class ImportMedisPeriod : Page, IRefresh, INotifyPropertyChanged, IConsoleMessage, IStartablePage
    {
        public ImportMedisPeriod()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += async (a, b) =>
            {
                MainWindow.Wait(true);
                await Task.Run(() => LoadExistsFromMedis());
                ConsoleMessage?.Invoke($"    Загружено строк: {DataRowSource?.Count()}");
                MainWindow.Wait();
            };

            /*Task.Run(() => */
            //LoadExistsFromMedis();
        }

        public DateTime ControlDate { get; set; } = DateTime.Now.AddDays(-30); // стандарт - за месяц до сегодня
        public IEnumerable<Movebook> DataRowSource { get; private set; } = new List<Movebook>();
        public object SelectedDataItem { get; set; }

        public bool CanStart => MedisContext.IsMedisonnectionEnable;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event common.RefreshEventHandler RefreshNotify;
        public event ConsoleMessageHandler ConsoleMessage;

        public void Refresh()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Импорт выбранных визитов в базу Castor
        /// </summary>
        private void SaveSelectedInCastor(object sender, RoutedEventArgs e)
        {
            try
            {
                using CastorContext castorContext = new CastorContext();
                castorContext.Movebooks.AddRange(DataRowSource);
                castorContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }


        /// <summary>
        /// Загружает визиты пациентов находящихся в отделении, в Root - поступление в п/п, оттуда - дата начала госпитализации
        /// </summary>
        private async Task LoadExistsFromMedis()
        {
            try
            {
                // список номеров и/б уже загруженных в базу Кастор
                using CastorContext castorContext = new CastorContext();
                List<long> castorIds = castorContext.Movebooks
                    .Select(x => x.Card_Id)
                    .ToList();


                using MedisContext medisContext = new MedisContext();

                // список визитов пациентов нваходящихся в настоящее время в отделении исключая номера историй уже заруженных в базу (по castorIds)
                List<visit> __DataRowSource = medisContext.visit
                    .Where(v => v.depid == Settings.Default.LastSelectedDepId && !v.dat1.HasValue) // Только находящиеся в отделении
                    .Include(v => v.Root) // чаще всего это поступление в п/п
                    .Include(v => v.Patient)  // привязка пациента
                    .ThenInclude(p => p.Diagnoses.Where(d => d.Diagnos.code.StartsWith("F"))) // диагнозы пациента
                    .ThenInclude(d => d.Diagnos)
                    .ToList()
                    .ExceptBy(castorIds, v => v.num)
                    .ToList();

                // для каждого полученного визита создается запись в movebook
                List<Movebook> movebooks = new List<Movebook>();
                foreach (visit vis in __DataRowSource)
                {
                    Movebook movebook = new Movebook();
                    movebook.Fio = vis.Patient?.fullname ?? string.Empty;
                    movebook.Datein = DateOnly.FromDateTime(vis.order_dat.Value); //vis.dat.HasValue ? DateOnly.FromDateTime(vis.dat.Value) : null;
                    movebook.Ordered = 0;
                    movebook.Card_Id = vis.num;
                    movebook.Patientid = vis?.Patient?.num;
                    movebook.Birthdate = DateOnly.FromDateTime(vis.Patient.birthdate.Value);
                    movebook.Visitid = vis?.keyid;
                    movebook.Dsin = vis.Patient.CurrentDs.Diagnos.code;

                    //if (vis.dat1.HasValue)
                    //{
                    //    movebook.Dateout = vis.dat1.HasValue ? DateOnly.FromDateTime(vis.dat1.Value) : null;
                    //    diagnos? dsOut = medis.diagnos
                    //        .Where(d => d.keyid == vis.Patient.CurrentDs.diagid)?.First();
                    //    movebook.Dsout = dsOut?.code;
                    //}

                    // проверка повторности поступления - поиск уже закрытых карт для пациента в этом учреждении
                    movebook.Second = medisContext.visit
                        .Include(v => v.Patient).Where(v => v.Patient.num == vis.Patient.num)
                        .Include(v => v.Dep)
                        .Where(v => v.Dep.rootid == Settings.Default.RootDepartmentId && v.dat1.HasValue && v.vistype == 102 && v.num != vis.num) // root=ДПБ и дата выписки заполнена и поступление в п/п и номер истории не равен загружаемому
                        .Count() > 0
                        ;

                    movebooks.Add(movebook);
                }

                DataRowSource = movebooks;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataRowSource)));

            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }

        private void ShowAllDiagnosis(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedDataItem is Movebook book)
                {
                    using MedisContext medis = new MedisContext();
                    ICollection<patdiag> dss = medis.patdiag
                        .Where(p => p.patientid == book.Patientid)
                        .Include(p => p.Diagnos)
                        .ToList();

                    DataGrid dataGrid = new DataGrid() { Language = XmlLanguage.GetLanguage("ru-RU") };
                    dataGrid.ItemsSource = dss;
                    Window window = new Window();
                    window.Content = dataGrid;
                    window.Show();
                }
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }

        /// <summary>
        /// отбирает все визиты пациента в указанное учреждение. Всключая закрытые истории и поступления в п/п
        /// </summary>
        private void ShowAllVisistsForPatient(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedDataItem is Movebook _book)
                {
                    using MedisContext medis = new MedisContext();

                    ICollection<visit> dss = medis.visit
                        .Where(v => v.patientid == _book.Patientid)
                        .Include(v => v.Dep)
                        .Where(w => w.Dep.rootid == Settings.Default.RootDepartmentId)
                        .ToList();

                    DataGrid dataGrid = new DataGrid() { Language = XmlLanguage.GetLanguage("ru-RU") };
                    dataGrid.ItemsSource = dss;
                    Window window = new Window();
                    window.Content = dataGrid;
                    window.Show();
                }
            }
            catch (Exception ex)
            {
                Message.ShowPopup(ex.Message);
            }
        }

        public void SaveOnCloseApplication()
        {
        }
    }

}
