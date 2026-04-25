using Castor.database;
using Castor.database.tab_medis;
using Castor.gui;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.gui.pages;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private StackPanel _ExtraStack = null;
        private static MainWindow _static_instance;

        public MainWindow()
        {
            _static_instance = this;
            InitializeComponent();
            new MenuLoader(CentralMenu).MenuItemRise += MainWindow_MenuItemRise;
            DataContext = this;
            Title = $"КНИГА ДВИЖЕНИЯ   :  {Depname}";

            // проверка сущестования файла БД для текущего отделения
            if (!File.Exists(Settings.Default.sqliteConnection))
            {
                // если file не существует, запрос отделения и пользователя
                new SelectUser().ShowDialog();
            }

            //// check department access
            //if (Settings.Default.LastSelectedDep != 2704 &&
            //    Settings.Default.LastSelectedDep != 2707)
            //{
            //    MessageBox.Show("Ваше отделение не имеет права использовать приложение", "Контроль", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

            // BACKUP
            new CastorContext().Backup();

            ContentRendered += async (o, e) =>
            {
                try
                {
                    Cursor = Cursors.Wait;
                    // load page according SettingsCheet.Default.StartLoadedPage
                    if (!Settings.Default.StartLoadingPage.IsNullOrEmpty())
                        MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = Settings.Default.StartLoadingPage });
                    Cursor = Cursors.Arrow;

                    WidgetsExtraInitialize();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDbName)));

                }
                catch { }
            };

            Closed += (a, b) =>
            {
                if (!string.IsNullOrWhiteSpace(CentralFrame?.Content?.GetType().ToString())
                    && CentralFrame?.Content is IStartablePage)
                {
                    Settings.Default.StartLoadingPage = CentralFrame.Content?.GetType().ToString();
                    Settings.Default.Save();
                }
            };
        }

        public static MainWindow Instance => _static_instance;
        public event PropertyChangedEventHandler? PropertyChanged;
        public string CurrentDbName => $"Data Source={Settings.Default.sqliteConnection}";
        public string Depname => Settings.Default.LastSelectedDepName;

        private void WidgetsExtraInitialize()
        {
            _ExtraStack = new StackPanel() { Orientation  = Orientation.Vertical };
            Grid.SetColumn(_ExtraStack, 0);
            MainFrameGrid.Children.Add(_ExtraStack);

            // get FullName of all Types contains lexem `Widget` in name
            var interfaces = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.FullName.Contains("Widget") && !t.IsNested);
            

            // create objects Wodgets from Parameter ExtraWidgets and add they into _ExtraStack
            foreach (var _widget in interfaces)
            {
                object WidgetObject = Activator.CreateInstance(_widget);
                _ExtraStack.Children.Add(WidgetObject as UIElement);
                _ExtraStack.Children.Add(new Separator());
            }
        }

        private void MainWindow_MenuItemRise(CastorMenuItem _castorMenuItem)
        {

            if (_castorMenuItem?.ClassName is string _className &&    // check CommandParameter is string
                Type.GetType(_className) is Type _class)             // try get Type from string
            {

                object? activeCreatedObject =
                    _class.GetConstructor([typeof(object)]) != null ? Activator.CreateInstance(_class, _castorMenuItem.Parameter) :
                    _class.GetConstructor([typeof(MainWindow)]) != null ? Activator.CreateInstance(_class, this) :
                    Activator.CreateInstance(_class);

                if (activeCreatedObject is IConsoleMessage obj) obj.ConsoleMessage += (message) => ConsoleMessage.Text = message;
                if (activeCreatedObject is IRun _objectIRun) _objectIRun.Run();
                if (activeCreatedObject is IDialog _objectDialog) _objectDialog.Show();
                if (activeCreatedObject is Page) CentralFrame.Content = activeCreatedObject;
                if (activeCreatedObject is ISwithPage swp) swp.SwitchPage += SwitchFramePage;
                if (activeCreatedObject is IMainStatusBar msb) msb.PrintStatusMessage += PrintStatusMessage;
                if (activeCreatedObject is IRefresh refh) refh.RefreshNotify += Refh_RefreshNotify;
            }
        }

        private void Refh_RefreshNotify(params string[] classes)
        {
            foreach (var item in _ExtraStack?.Children)
            {
                if (item is IRefresh irf && classes.Any(c => c == item.GetType().FullName))
                {
                    irf.Refresh();
                }
            }

            if(CentralFrame.Content is IRefresh rf && classes.Any(c => c == rf.GetType().FullName))
            {
                rf.Refresh();
            }
        }

        private void PrintStatusMessage(string message, string barName)
        {
            foreach (var item in MainStatusBar.Items)
            {
                if (item is TextBlock _iTextBlock && _iTextBlock.Name == barName)
                {
                    _iTextBlock.Text = message;
                }
            }
        }

        private void SwitchFramePage(string className, object param)
        {
            MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = className, Parameter = param });
        }

        public static void Wait(bool wait=false)
        {
            if (wait) Instance.Cursor = Cursors.Wait;
            else Instance.Cursor = Cursors.Arrow;
        }
       


    }
}