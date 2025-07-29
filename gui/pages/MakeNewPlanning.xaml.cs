using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Castor.gui.pages
{
    /// <summary>
    /// Логика взаимодействия для MakeNewPlanning.xaml
    /// </summary>
    public partial class MakeNewPlanning : Window, IDialog, INotifyPropertyChanged
    {
        private CastorContext _context;
        public MakeNewPlanning(CastorContext castorContext, object _visit)
        {
            _context = castorContext;
            Visit = (visit?)_visit;
            InitializeComponent();
            DataContext = this;

            Planns = castorContext.DictPlannings.ToList();

            Planning = new planning();
            Planning.patientid = Visit.patientid.Value;
            Planning.docdepid = Visit.doctorid.Value;
            Planning.depid = Visit.depid.Value;
            Planning.visitid = Visit.keyid;
            Planning.start_date = Visit.dat.Value;
            _context.Plannings.Add(Planning);
        }

        public MakeNewPlanning(CastorContext castorContext, planning planning)
        {
            _context = castorContext;
            InitializeComponent();
            DataContext = this;
            Planns = castorContext.DictPlannings.ToList();

            Planning = planning;
        }

        public visit? Visit { get; set; }
        public ICollection<dictionary>? Planns { get; set; }
        public planning Planning { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dictionary sel_dictionary = (dictionary)((ComboBox)sender).SelectedItem;
            Planning.next_date = Planning.start_date?.AddDays(sel_dictionary.period);
            if (Planning.next_date.Value.DayOfWeek==DayOfWeek.Saturday) Planning.next_date = Planning.next_date.Value.AddDays(-1);
            if (Planning.next_date.Value.DayOfWeek == DayOfWeek.Sunday) Planning.next_date = Planning.next_date.Value.AddDays(-2);

            Planning.description = sel_dictionary.description;
            Planning.cycles = Planning.cycles + 1;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Planning)));
        }

        private void MakeNextCycle(object sender, RoutedEventArgs e)
        {
            if(Planning.DaysToNext>0)
            {
                if (MessageBox.Show($"До контрольного срока осталось {Planning.DaysToNext} дней.\nВы уверены, что желаете продлить цикл раньше срока?", "Цикл", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
                    
            }
            dictionary sel_dictionary = (dictionary)DictionaryCombo.SelectedItem;
            Planning.start_date = Planning.next_date != null ?
                Planning.next_date > DateTime.Today ? DateTime.Now : Planning.next_date
                : Planning.start_date;
            Planning.next_date = Planning.start_date?.AddDays(sel_dictionary.period);
            if (Planning.next_date.Value.DayOfWeek == DayOfWeek.Saturday) Planning.next_date = Planning.next_date.Value.AddDays(-1);
            if (Planning.next_date.Value.DayOfWeek == DayOfWeek.Sunday) Planning.next_date = Planning.next_date.Value.AddDays(-2);
            Planning.cycles = Planning.cycles + 1;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Planning)));
        }

        private void NextDayChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Planning)));
        }

        private void ClosePlan(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Действиельно завершить план напоминаний?", "Цикл", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Planning.executed = true;
                SaveToCastorBase(null, null);
            }
        }

        private void SaveToCastorBase(object sender, RoutedEventArgs e)
        {
            _context.SaveChanges();
            Close();
        }
    }
}
