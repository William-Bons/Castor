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

            var viewModel = new PatientSearchViewModel();
            DataContext = viewModel;

        }
    }

}
