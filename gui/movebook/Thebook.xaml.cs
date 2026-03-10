using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для Thebook.xaml
    /// </summary>
    public partial class Thebook : Page, INotifyPropertyChanged, IRefresh, IStartablePage
    {
        private CastorContext context;
        private bool need_save = false;

        public Thebook()
        {
            InitializeComponent();
            DataContext = this;
            Task.Run(() => Load(new DatePeriod()));
        }

        public ICollection<Movebook> LoadedData { get; private set; }
        public Visibility SaveButtonVisible => need_save ? Visibility.Visible : Visibility.Collapsed;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event common.RefreshEventHandler RefreshNotify;

        private void PatientsTable_CellEditEnding(object sender, EventArgs e)
        {
            need_save = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonVisible)));
        }

        private async Task Load(DatePeriod Period)
        {
            if (context != null)  context.Dispose();
            context = new CastorContext();

            if (Period.Set)
            {
                LoadedData = context.Movebooks
                    .Where(x => (x.Datein >= DateOnly.FromDateTime(Period.Start) && x.Datein <= DateOnly.FromDateTime(Period.End)) ||
                    (x.Dateout >= DateOnly.FromDateTime(Period.Start) && x.Dateout <= DateOnly.FromDateTime(Period.End)))
                    .ToList();
            }
            else
            {
                LoadedData = context.Movebooks.ToList();
                ;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadedData)));
            

        }

        private async void NeedRefreshTable(object sender, EventArgs e)
        {
            need_save = false;

            LoadedData.Where(x => x.Dateout == DateOnly.MinValue).ToList().ForEach(x => x.Dateout = null);
            LoadedData.Where(x => x.Fss == DateOnly.MinValue).ToList().ForEach(x => x.Fss = null);
            //LoadedData.Where(x => x.Forced == DateOnly.MinValue).ToList().ForEach(x => x.Forced = null);

            context.SaveChanges(true);
            
            await Task.Run(()=> Load(new DatePeriod()));
            RefreshNotify?.Invoke("Castor.gui.pages.FssControl", "Castor.gui.pages.Weekmove", "Castor.gui.pages.ForceControl");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonVisible)));
        }

        private async void DisorderPatient(object sender, EventArgs e)
        {
            if(PatientsTable.SelectedItem is Movebook mvb)
            {
                Disorder disorder = new Disorder(mvb);
                disorder.ShowDialog();
                await Task.Run(() => Load(new DatePeriod()));
                RefreshNotify?.Invoke("Castor.gui.pages.FssControl", "Castor.gui.pages.Weekmove");
            }
        }

        private void SaveInXml(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            MonthReportHtml monthReportHtml = new MonthReportHtml(SelectDatePeriod.GetStandard());
            NavigationService.Navigate(monthReportHtml.DisplayReportAsHTML());
            Cursor = Cursors.Arrow;
        }

        private void LoadPatient(object sender, EventArgs e)
        {
            CreateNew createNew = new CreateNew(sender is Button ? null : PatientsTable.SelectedItem);
            createNew.ShowDialog();
            NeedRefreshTable(sender, e);
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Cursor = Cursors.Wait;
            DatePeriod dp = new DatePeriod();
            dp.Start = dpStart.SelectedDate.HasValue ? dpStart.SelectedDate.Value : dp.Start;
            dp.End = dpEnd.SelectedDate.HasValue ? dpEnd.SelectedDate.Value : dp.End;
            dp.Set = dp.Start > DateTime.MinValue && dp.End > DateTime.MinValue && dp.End>dp.Start;
            Task.Run(()=> Load(dp));
            Cursor = Cursors.Arrow;
        }

        private async void ImportList(object sender, RoutedEventArgs e)
        {
            //ImportMedisPeriod importMedisPeriod = new ImportMedisPeriod();
            //importMedisPeriod.ShowDialog();

            List<long?> visitIds;
            List<visit> visits = new List<visit>();

            Func<Task> __check = async () =>
            {
                try
                {


                    using (CastorContext castorContext = new CastorContext())
                    {
                        visitIds = castorContext.Movebooks.Select(m => m.Visitid).ToList();
                    }

                    using (MedisContext medisContext = new MedisContext())
                    {
                        /* Запрос к Медис*/
                        ICollection<dep>? depList = medisContext.dep // отделения
                            .Where(d => d.keyid == Settings.Default.LastSelectedDep) // где номер отделения = сохраненному в Settings
                            .Include(d => d.Visits.Where(v => !v.dat1.HasValue || (DateTime.Today.ToUniversalTime() - v.dat1).Value.Days < 8))
                            .ThenInclude(v => v.Patient)  // привязка пациента
                            .ThenInclude(p => p.Diagnoses) // привязка диагнозов пациента
                            .ThenInclude(d => d.Diagnos)  // привязка к диагнозам пациента дианоза из мкб
                            .ToList();

                        visits = depList.Select(d => d.Visits).First().ExceptBy(visitIds, v => v.keyid).ToList();
                    }

                    SelectObjectFromEnumerable soe = new SelectObjectFromEnumerable("Не загруженные", visits, "Patient.fullname", "Patient.age", "dat", "dat1", "Patient.CurrentDs.text");
                    soe.Selected += (a) =>
                    {
                        CreateNew createNew = new CreateNew(a);
                        createNew.ShowDialog();
                        NeedRefreshTable(sender, e);
                    };

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };
            await __check();
        }

        private async void DeleteRow(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить строку из базы данных?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                context.Movebooks.Remove(PatientsTable.SelectedItem as Movebook);
                context.SaveChanges();
                await Task.Run(() => Load(new DatePeriod()));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonVisible)));
                RefreshNotify?.Invoke("Castor.gui.pages.FssControl", "Castor.gui.pages.Weekmove");
            }
        }

        private void ShowStatGroup(object sender, RoutedEventArgs e)
        {
            try
            {
                var mb = PatientsTable.SelectedItem as Movebook;
                bool[] mass0 = mb.calc0(mb.Dsin);
                bool[] mass1 = mb.calc0(mb.Dsout);

                StringBuilder strOUT = new StringBuilder();
                strOUT.AppendLine("A B C D E F");
                foreach (var item in mass0)
                {
                    strOUT.AppendFormat("{0} ", item? 1:0);
                }
                strOUT.AppendLine();
                foreach (var item in mass1)
                {
                    strOUT.AppendFormat("{0} ", item? 1:0);
                }

                TextBlock textBlock = new TextBlock();
                textBlock.Text = strOUT.ToString();
                Window window = new Window() { MaxHeight = 120, MaxWidth = 150 };
                window.Content = textBlock;
                window.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
         }

        public void Refresh()
        {
        }

        private void HideAutoColumns(object sender, RoutedEventArgs e)
        {
            PatientsTable.AutoGenerateColumns = (sender as MenuItem).IsChecked;
        }

    }
}
