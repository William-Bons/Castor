using Castor.database;
using Castor.database.tab_medis;
using Castor.gui;
using Castor.gui.common;
using Castor.Properties;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
                Cursor = Cursors.Wait;

                // select current user
                if (Settings.Default.AskUserBeforeStart && Settings.Default.LastConnectedUserId <= 0 && Settings.Default.LastSelectedDep <= 0)
                {
                    MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = "Castor.gui.dialogs.SelectUser" });
                }

                // load page according Settings.Default.StartLoadedPage
                if (!Settings.Default.StartLoadingPage.IsNullOrEmpty())
                    MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = Settings.Default.StartLoadingPage });

                Cursor = Cursors.Arrow;
            };

            Closed += (a, b) =>
            {
                if (!string.IsNullOrWhiteSpace(CentralFrame?.Content?.GetType().ToString()))
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
                    _class.GetConstructor([typeof(MainWindow)]) != null ? Activator.CreateInstance(_class, this) :
                    _class.GetConstructor([typeof(object)]) != null ? Activator.CreateInstance(_class, _castorMenuItem.Parameter) :
                    Activator.CreateInstance(_class);

                if (activeCreatedObject is IConsoleMessage obj) obj.ConsoleMessage += (message) => Console.Print($"{message}");
                if (activeCreatedObject is IRun _objectIRun) _objectIRun.Run();
                if (activeCreatedObject is IDialog _objectDialog) _objectDialog.Show();
                if (activeCreatedObject is Page) CentralFrame.Content = activeCreatedObject;
                if (activeCreatedObject is ISwithPage swp) swp.SwitchPage += SwitchFramePage;
                if (activeCreatedObject is IMainStatusBar msb) msb.PrintStatusMessage += PrintStatusMessage;
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