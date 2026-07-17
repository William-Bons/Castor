using Castor.database;
using Castor.database.reports;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.commities;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.gui.find;
using Castor.gui.force;
using Castor.gui.pages;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для Theband.xaml
    /// </summary>
    public partial class Thebook : Page, IRefresh, IStartablePage, IConsoleMessage
    {
        public Thebook()
        {
            
            InitializeComponent();

            Loaded += async (a, b) =>
            {
                MainWindow.Wait(true);
                DataContext = new ThebookViewModel();
                await ((ThebookViewModel)DataContext).Load();
                ConsoleMessage?.Invoke($"    Загружено строк: {((ThebookViewModel)DataContext).LoadedData?.Count()}");
                Application.Current.Exit += SaveOnCloseApplication;
                MainWindow.Wait();
            };

            Unloaded += async (a, b) =>
            {
                // отписка от сохранения при выходе т.к. окно закрывается
                Application.Current.Exit -= SaveOnCloseApplication;
                MovebookDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                await ((ThebookViewModel)DataContext).Save();
            };
        }

       
        
        public bool CanStart => true;
        public event common.RefreshEventHandler? RefreshNotify;
        public event ConsoleMessageHandler? ConsoleMessage;

        public async  void Refresh()
        {
            await ((ThebookViewModel)DataContext).Load();
        }

        /// <summary>
        /// Сохранение измененных данных однократно при выходе из приложения
        /// </summary>
        public void SaveOnCloseApplication(object sender, EventArgs e)
        {
            MovebookDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            ((ThebookViewModel)DataContext)?.Save();
        }

        private void PatientsTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((ThebookViewModel)DataContext)?.EditMovebookItemWindowAsync();

        }

        private void OpenPMMCommand(object sender, RoutedEventArgs e)
        {
            ((ThebookViewModel)DataContext).OpenPMMWindow();
        }

        private void OpenCMCommand(object sender, RoutedEventArgs e)
        {
            ((ThebookViewModel)DataContext).OpenCommityWindow();
        }

        private void OpenOUTCommand(object sender, RoutedEventArgs e)
        {
            ((ThebookViewModel)DataContext)?.DisorderPatient();
        }
    }
}
