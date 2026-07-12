using Castor.database.tab_medis;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Input;

namespace Castor.gui.find
{
    public partial class PatientSearchWindow : Window, IDialog
    {
        public PatientSearchWindow()
        {
            InitializeComponent();

            var context = new MedisContext(); // твой DbContext
            var viewModel = new PatientSearchViewModel(context);
            DataContext = viewModel;

            // опционально: закрыть контекст при закрытии окна
            this.Closed += (s, e) => context.Dispose();
        }
    }

}
