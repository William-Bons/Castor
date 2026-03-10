using Castor.gui;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.IdentityModel.Tokens;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new MenuLoader(CentralMenu).MenuItemRise += MainWindow_MenuItemRise;

            ContentRendered += async (o, e) =>
            {
                try
                {
                    Cursor = Cursors.Wait;

                    // select current user
                    if (Settings.Default.AskUserBeforeStart && Settings.Default.LastConnectedUserId <= 0 && Settings.Default.LastSelectedDep <= 0)
                    {
                        MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = "Castor.gui.dialogs.SelectUser" });
                    }

                    // load page according SettingsCheet.Default.StartLoadedPage
                    if (!Settings.Default.StartLoadingPage.IsNullOrEmpty())
                        MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = Settings.Default.StartLoadingPage });

                    Cursor = Cursors.Arrow;
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

        private void MainWindow_MenuItemRise(CastorMenuItem _castorMenuItem)
        {

            if (_castorMenuItem?.ClassName is string _className &&    // check CommandParameter is string
                Type.GetType(_className) is Type _class)             // try get Type from string
            {

                object? activeCreatedObject =
                    _class.GetConstructor([typeof(object)]) != null ? Activator.CreateInstance(_class, _castorMenuItem.Parameter) :
                    _class.GetConstructor([typeof(MainWindow)]) != null ? Activator.CreateInstance(_class, this) :
                    Activator.CreateInstance(_class);

                if (activeCreatedObject is IConsoleMessage obj) obj.ConsoleMessage += (message) => Console.Print($"{message}");
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
            foreach (var item in CentralStack.Children)
            {
                if(item is IRefresh irf && classes.Where(c => c==item.GetType().FullName).Count()>0)
                {
                    irf.Refresh();
                }
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