using Castor.database.tab_medis;
using Castor.gui;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Castor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            Instance = this;
            DataContext = this;
            InitializeComponent();

            new MenuLoader(CentralMenu).MenuItemRise += MainWindow_MenuItemRise;

            // инициализация MainWindow and ExtraWidgets
            WidgetsExtraInitialize();

            // load page according SettingsCheet.Default.StartLoadedPage
            if (!Settings.Default.StartLoadingPage.IsNullOrEmpty())
                MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = Settings.Default.StartLoadingPage });

            Closed += (a, b) =>
            {
                // сохранение открытой страницы 
                if (!string.IsNullOrWhiteSpace(CentralFrame?.Content?.GetType().ToString())
                    && CentralFrame?.Content is IStartablePage)
                {
                    Settings.Default.StartLoadingPage = CentralFrame.Content?.GetType().ToString();
                    Settings.Default.Save();
                }

                Application.Current?.Shutdown();
            };
        }

        public static MainWindow Instance { get; private set; } = null!;
        public string CurrentDbName => $"Data Source={Settings.Default.sqliteConnection}";
        public string Depname => Settings.Default.LastSelectedDepName;
        public Brush MedisEnabled =>
            MedisContext.IsMedisonnectionEnable ? Brushes.Green : Brushes.Red;

        private void WidgetsExtraInitialize()
        {
            var _ExtraStack = new StackPanel() { Orientation = Orientation.Vertical };
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

        /// <summary>
        /// создает объект класса по имени загруженного из параметра меню. активирует объект
        /// </summary>
        private void MainWindow_MenuItemRise(CastorMenuItem _castorMenuItem)
        {

            if (_castorMenuItem?.ClassName is string _className &&    // check CommandParameter is string
                Type.GetType(_className) is Type _class)             // try get Type from string
            {

                object? activeCreatedObject =
                    _class.GetConstructor([typeof(object)]) != null ? Activator.CreateInstance(_class, _castorMenuItem.Parameter) :
                    _class.GetConstructor([typeof(MainWindow)]) != null ? Activator.CreateInstance(_class, this) :
                    Activator.CreateInstance(_class);

                if (activeCreatedObject is IConsoleMessage obj) obj.ConsoleMessage += (message) => ConsoleMessage.Text = message;   //todo оценить необходимость
                if (activeCreatedObject is IDialog _objectDialog) _objectDialog.Show();                                             //todo открывает как окно надо добавить вариант как диалог
                if (activeCreatedObject is ISwithPage swp) swp.SwitchPage += SwitchFramePage;                                       // позволяет переключить на другую страницу
                if (activeCreatedObject is IMainStatusBar msb) msb.PrintStatusMessage += PrintStatusMessage;                        // позволяет вывести сообщение на StatusBar
                if (activeCreatedObject is IRefresh refh) refh.RefreshNotify += Refh_RefreshNotify;                                 // позволяет обновить другой дочерний элемент MainFrame

                // служебные 
                if (activeCreatedObject is IRun _objectIRun) _objectIRun.Run();                                                     // тест интерфейс запускает функцию Run

                // основная загрузка страницы
                if (activeCreatedObject is IStartablePage isp && isp.CanStart)
                {
                    CentralFrame.Content = activeCreatedObject;
                }

            }

            // Обновление некоторых визуальных информационных знаков при переходах по меню
            // -- знак досупности медис
            OnPropertyChanged(nameof(MedisEnabled));
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void Refh_RefreshNotify(params string[] classes)
        {
            FindVisualChildren<UIElement>(this).OfType<IRefresh>()      // find all children UIElement interfaces IRefresh
                .Where(x => classes.Contains(x.GetType().FullName))     // filter by names in classes parameter
                .ToList()
                .ForEach(i => i.Refresh());                             // call Refresh method for all found objects
        }

        private void PrintStatusMessage(string message, string barName)
        {
            MainStatusBar.Items.OfType<TextBlock>()
                .First(t => t.Name == barName)
                .Text = message;
        }

        private void SwitchFramePage(string className, object param)
        {
            MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = className, Parameter = param });
        }

        public static void Wait(bool wait = false)
        {
            if (wait) Instance.Cursor = Cursors.Wait;
            else Instance.Cursor = Cursors.Arrow;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


    }
}