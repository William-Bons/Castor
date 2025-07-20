using Castor.database;
using Castor.database.tables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для CreateNewUser.xaml
    /// </summary>
    public partial class CreateNew : Window
    {
        public delegate void DialogOKHandler();
        public event DialogOKHandler DialogOK;
        public CreateNew(CastorCommonContext Db, MetaTable TableObject)
        {
            InitializeComponent();
            DataContext = this;
            new DbTableBinder(Db, TableObject, Face);
        }

        private void SaveInDb(object sender, RoutedEventArgs e)
        {
            DialogOK?.Invoke();
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
