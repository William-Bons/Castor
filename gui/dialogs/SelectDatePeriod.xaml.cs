using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.movebook;
using System.Windows;
using System.Windows.Input;

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

            OkCommand = new RelayCommand(_ => OnOk());
            CancelCommand = new RelayCommand(_ => OnCancel());
            CloseRequest += (a, b) => Close();
        }

        // Event для закрытия (стандартный паттерн MVVM)
        public event EventHandler? CloseRequest;
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public MoveFilter Period { get; set; } = new MoveFilter();

        public static new MoveFilter Show()
        {
            SelectDatePeriod selectDatePeriod = new SelectDatePeriod();
            selectDatePeriod.ShowDialog();
            return selectDatePeriod.Period;
        }

        private void OnOk()
        {
            Period.DatesSet = true;
            // Если всё ок — уведомляем окно, что можно закрываться
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnCancel()
        {
            Period = new MoveFilter();
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

    }
}
