using Castor.database.tables;
using System.Windows;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для SelectDatePeriod.xaml
    /// </summary>
    /// 
    

    public partial class SelectDatePeriod : Window
    {
        public SelectDatePeriod()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static new DatePeriod Show()
        {
            SelectDatePeriod selectDatePeriod = new SelectDatePeriod();
            selectDatePeriod.ShowDialog();
            DatePeriod datePeriod = new DatePeriod();
            datePeriod.Start = selectDatePeriod.SelectedStart;
            datePeriod.End = selectDatePeriod.SelectedEnd;
            datePeriod.Set = true;
            return datePeriod;
        }

        public static DatePeriod GetStandard()
        {
            SelectDatePeriod selectDatePeriod = new SelectDatePeriod();
            DatePeriod datePeriod = new DatePeriod();
            datePeriod.Start = selectDatePeriod.SelectedStart;
            datePeriod.End = selectDatePeriod.SelectedEnd;
            return datePeriod;
        }

        public DateTime SelectedStart { get; set; } = DateTime.Parse($"{21}.{(DateTime.Today.Month > 1 ? DateTime.Today.Month - 1 : 12)}.{DateTime.Today.Year}");
        public DateTime SelectedEnd { get; set; } = DateTime.Parse($"{20}.{DateTime.Today.Month}.{DateTime.Today.Year}");
    }
}
