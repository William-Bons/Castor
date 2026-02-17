using Castor.database;
using Castor.database.tables;
using Castor.gui.dialogs;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для Thebook.xaml
    /// </summary>
    public partial class Thebook : Page, INotifyPropertyChanged
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

        private void PatientsTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            need_save = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonVisible)));
        }

        private async Task Load(DatePeriod Period)
        {
            if (context != null) context.Dispose();

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

        private void NeedRefreshTable(object sender, System.Windows.RoutedEventArgs e)
        {
            need_save = false;
            context.SaveChanges(true);
            Task.Run(()=> Load(new DatePeriod()));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SaveButtonVisible)));
        }

        private void DisorderPatient(object sender, EventArgs e)
        {
            if(PatientsTable.SelectedItem is Movebook mvb)
            {
                Disorder disorder = new Disorder(mvb);
                disorder.ShowDialog();
            }
        }

        private void SaveInXml(object sender, RoutedEventArgs e)
        {
            
            DatePeriod datePeriod = SelectDatePeriod.Show();

            Cursor = Cursors.Wait;
            MonthReportHtml monthReportHtml = new MonthReportHtml(datePeriod);
            monthReportHtml.DisplayReportAsHTML();
            Cursor = Cursors.Arrow;
        }

        private void LoadPatient(object sender, RoutedEventArgs e)
        {
            CreateNew createNew = new CreateNew();
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
    }
}
