using Castor.gui.common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для Theband.xaml
    /// </summary>
    public partial class Thebook : Page, IRefresh, IStartablePage, IConsoleMessage
    {
        private ThebookViewModel _model = null!;
        public Thebook()
        {
            _model = new ThebookViewModel();

            InitializeComponent();

            Loaded += async (a, b) =>
            {
                MainWindow.Wait(true);
                DataContext = _model;
                await _model.Load();
                ConsoleMessage?.Invoke($"    Загружено строк: {_model.LoadedData?.Count()}");
                Application.Current.Exit += SaveOnCloseApplication;
                MainWindow.Wait();
            };

            Unloaded += async (a, b) =>
            {
                // отписка от сохранения при выходе т.к. окно закрывается
                Application.Current.Exit -= SaveOnCloseApplication;
                MovebookDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                await _model.Save();
            };
        }



        public bool CanStart => true;
        public event common.RefreshEventHandler? RefreshNotify;
        public event ConsoleMessageHandler? ConsoleMessage;

        public async void Refresh()
        {
            await _model.Load();
        }

        /// <summary>
        /// Сохранение измененных данных однократно при выходе из приложения
        /// </summary>
        public void SaveOnCloseApplication(object sender, EventArgs e)
        {
            MovebookDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            _model?.Save();
        }

        private void PatientsTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _model?.EditMovebookItemWindowAsync();

        }

        private void OpenPMMCommand(object sender, RoutedEventArgs e)
        {
            _model.OpenPMMWindow();
        }

        private void OpenCMCommand(object sender, RoutedEventArgs e)
        {
            _model.OpenCommityWindow();
        }

        private void OpenOUTCommand(object sender, RoutedEventArgs e)
        {
            _model?.DisorderPatient();
        }
    }
}
