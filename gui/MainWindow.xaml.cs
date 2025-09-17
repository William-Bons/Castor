using Castor.database;
using Castor.database.tab_medis;
using Castor.gui;
using Castor.gui.common;
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
        private CastorContext? DatabaseContext;
        public MainWindow()
        {
            InitializeComponent();
            new MenuLoader(CentralMenu).MenuItemRise += MainWindow_MenuItemRise;

            ContentRendered += async (o, e) =>
            {
                Cursor = Cursors.Wait;

                // THIS! Connection program to database
                await Task.Run(() =>
                {
                    DatabaseContext = CastorContext.Get();
                });
                Console.Print($"conncted: {DatabaseContext?.Variant}");

                // Test Connection To Medis Database
                using (MedisContext mc = new MedisContext())
                {
                    Console.Print(mc.Variant);
                }

                // load Kurwa
                MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = "Castor.Kurwa" });

                // select current user
                MainWindow_MenuItemRise(new CastorMenuItem() { ClassName = "Castor.gui.dialogs.SelectUser" });

                Cursor = Cursors.Arrow;
            };
        }

        private void MainWindow_MenuItemRise(CastorMenuItem _castorMenuItem)
        {

            if (_castorMenuItem?.ClassName is string _className &&    // check CommandParameter is string
                Type.GetType(_className) is Type _class)             // try get Type from string
            {

                object? activeCreatedObject =
                    _class.GetConstructor([typeof(CastorContext)]) != null ? Activator.CreateInstance(_class, DatabaseContext) :
                    _class.GetConstructor([typeof(CastorContext), typeof(object)]) != null ? Activator.CreateInstance(_class, DatabaseContext, _castorMenuItem.Parameter) :
                    _class.GetConstructor([typeof(MainWindow)]) != null ? Activator.CreateInstance(_class, this) :
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