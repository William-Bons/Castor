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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Castor.gui.bandbook
{
    /// <summary>
    /// Логика взаимодействия для SocialPage.xaml
    /// </summary>
    public partial class SocialPage : UserControl
    {
        public SocialPage(Bandbook bandbookSelected)
        {
            Bandline = bandbookSelected;
            InitializeComponent();
            DataContext = this;
        }

        public Bandbook Bandline { get; set; }
    }
}
