using Castor.database.tab_medis;
using Castor.gui.common;
using System.Windows.Controls;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для ImportMedisPeriod.xaml
    /// </summary>
    public partial class ImportMedisPeriod : Page, IRefresh, IConsoleMessage, IStartablePage
    {
        public ImportMedisPeriod()
        {
            InitializeComponent();

            ImportMedisModel importMedisModel = new();
            DataContext = importMedisModel;

            Loaded += async (a, b) =>
            {
                ConsoleMessage?.Invoke($"    Загружено строк: {importMedisModel.DataRowSource?.Count()}");
            };
        }



        public bool CanStart => MedisContext.IsMedisonnectionEnable;
        public event common.RefreshEventHandler? RefreshNotify;
        public event ConsoleMessageHandler? ConsoleMessage;

        public void Refresh()
        {
            throw new NotImplementedException();
        }





    }

}
