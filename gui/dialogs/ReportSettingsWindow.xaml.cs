using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Castor.gui.dialogs
{



    public partial class ReportSettingsWindow : Window, INotifyPropertyChanged
    {
        private int? selectedRange;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int? SelectedRange 
        { 
            get => selectedRange;
            set
            {
                selectedRange = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(SelectedRange)));
            }
        }

        public ReportSettingsWindow()
        {
            InitializeComponent();
            InitializeMonthRanges();
        }

        private void InitializeMonthRanges()
        {
            var culture = CultureInfo.GetCultureInfo("ru-RU");

            var ranges = Enumerable.Range(1, 12)
                .Select(startMonth =>
                {
                    var endMonthRaw = startMonth + 6;
                    var endMonth = endMonthRaw > 12 ? endMonthRaw - 12 : endMonthRaw;

                    var startName = culture.DateTimeFormat.GetMonthName(startMonth);
                    var endName = culture.DateTimeFormat.GetMonthName(endMonth);

                    return new MonthRange
                    {
                        StartMonth = startMonth,
                        EndMonth = endMonth,
                        DisplayText = $"{startName} — {endName}"
                    };
                })
                .ToList();

            MonthRangeComboBox.ItemsSource = ranges;
            MonthRangeComboBox.DisplayMemberPath = "DisplayText";
            MonthRangeComboBox.SelectedValuePath = "StartMonth";
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (MonthRangeComboBox.SelectedItem is MonthRange range)
            {
                SelectedRange = range.StartMonth;
                DialogResult = true;      // если окно как диалог
                Close();
            }
            else
            {
                MessageBox.Show("Выберите период из списка.", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedRange = null;
            DialogResult = false;
            Close();
        }
    }

    public class MonthRange
    {
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public string DisplayText { get; set; } = "";
    }

}

