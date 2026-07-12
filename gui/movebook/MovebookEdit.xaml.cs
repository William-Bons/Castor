using Castor.database.tab_medis;
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

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для MovebookEdit.xaml
    /// </summary>
    public partial class MovebookEdit : Window
    {
        public MovebookEdit(object? scaff, int selectedControl=0)
        {
            InitializeComponent();

            Loaded += (a, b) =>
            {
                var model = new MovebookEditModel(scaff);
                if (selectedControl == 1) model.PrepareDisorder(); // подготовка данныз под выписку
                DataContext = model;
                model.RequestClose += (a,b) => Close();
                MainTabControl.SelectedIndex = selectedControl;
            };
        }


    }
}
