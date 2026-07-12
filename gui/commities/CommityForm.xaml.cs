using Castor.gui.common;
using System.Windows;

namespace Castor.gui.commities
{
    public partial class CommityForm : Window, IDialog
    {
        public CommityForm(database.tables.Movebook? selected)
        {
            InitializeComponent();
            // Устанавливаем контекст данных для работы привязок (Binding)
            this.DataContext = new CommityViewModel();
        }
    }
}
