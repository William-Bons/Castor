using Castor.database.tab_medis;
using Castor.gui.common;
using System.Windows.Controls;

namespace Castor.gui.force
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class MonthsPanel : Page, IStartablePage
    {
        public MonthsPanel()
        {
            InitializeComponent();

            // нужно так чтобы класс модели вызывался только если страница загрузилась т.к. там подключение к медис 
            Loaded += async (a, b) =>
            {
                DataContext = new MonthsPanelViewModel();
                await ((MonthsPanelViewModel)DataContext).RefreshAsync();
            };
        }

        public bool CanStart => MedisContext.IsMedisonnectionEnable;


    }
}
