using Castor.database;
using Castor.database.tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Castor.gui.force
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ForcedTreeWindow : Window
    {
        private readonly ForcedViewModel _viewModel;

        public ForcedTreeWindow(Movebook? movebook)
        {
            InitializeComponent();
            _viewModel = new ForcedViewModel(movebook);
            DataContext = _viewModel;
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ForcedNode node)
            {
                _viewModel.SelectedItem = node.Data;
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
