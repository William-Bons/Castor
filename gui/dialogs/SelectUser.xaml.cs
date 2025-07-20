using Castor.database;
using Castor.database.tables;
using Microsoft.EntityFrameworkCore;
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

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для SelectUser.xaml
    /// </summary>
    public partial class SelectUser : Window
    {
        private CastorCommonContext Database;

        public List<User> Users { get; set; }
        public User LastConnectedUser { get; set; }

        public SelectUser(CastorCommonContext Database)
        {
            this.Database = Database;
            Users = Database.Users.ToList();
            InitializeComponent();
            DataContext = this;
        }

        private void ConnectAndRegisterUser(object sender, RoutedEventArgs e)
        {
            User.SetCurrent(LastConnectedUser);
            Close();
        }

        private void SetupAnimaConnectionString(object sender, RoutedEventArgs e)
        {

        }

        private void ResetSavedUsers(object sender, RoutedEventArgs e)
        {

        }
    }
}
