using Castor.database.tables;
using System.Windows;

namespace Castor.gui.force
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ForcedTreeWindow : Window
    {
        private readonly ForcedViewModel _viewModel;

        public ForcedTreeWindow(Movebook movebook)
        {
            InitializeComponent();
            _viewModel = new ForcedViewModel(movebook);
            DataContext = _viewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
