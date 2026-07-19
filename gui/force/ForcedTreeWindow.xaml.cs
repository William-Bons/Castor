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

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Forced node)
            {
                _viewModel.SelectedItem = node;
            }
            else
            {
                _viewModel.SelectedItem = null;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
