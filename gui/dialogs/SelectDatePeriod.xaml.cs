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

        public DatePeriod Period { get; set; } = new DatePeriod(true);

        public static new DatePeriod Show()
        {
            SelectDatePeriod selectDatePeriod = new SelectDatePeriod();
            selectDatePeriod.ShowDialog();
            return selectDatePeriod.Period;
        }

        
    }
}
