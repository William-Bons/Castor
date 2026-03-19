using Castor.database;
using Castor.database.tab_medis;
using Castor.gui;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.gui.pages;
using Castor.Properties;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
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
        public MainWindow()
        {
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

                    ExtraInitialize();
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

        public string CurrentDbName => $"Data Source={Settings.Default.sqliteConnection}";
        public string Depname
        {
            get
            {
                try
                {
                    using (MedisContext medis = new MedisContext())
                    {
                        return
                            medis.dep.Where(d => d.keyid == Settings.Default.LastSelectedDep)?.First().text;
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ExtraInitialize()
        {
            _ExtraStack = new StackPanel() { Orientation  = Orientation.Vertical };
            Grid.SetColumn(_ExtraStack, 1);
            MainFrameGrid.Children.Add(_ExtraStack);

            // if ShowWeek
            _ExtraStack.Children.Add(new Weekmove());
            _ExtraStack.Children.Add(new Separator());

            // if FSS
            _ExtraStack.Children.Add(new FssWidget());
            _ExtraStack.Children.Add(new Separator());

            // if FORCED
            _ExtraStack.Children.Add(new ForceWidget());
            _ExtraStack.Children.Add(new Separator());

            _ExtraStack.Children.Add(new UnvlWidget());
            _ExtraStack.Children.Add(new Separator());
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
                if (item is IRefresh irf && classes.Where(c => c == item.GetType().FullName).Count() > 0)
                {
                    irf.Refresh();
                }
            }

            if(CentralFrame.Content is IRefresh rf && classes.Where(c => c == rf.GetType().FullName).Count() > 0)
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

       


    }
}