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

namespace Castor.gui.force
{
    /// <summary>
    /// Логика взаимодействия для Forcepage.xaml
    /// </summary>
    public partial class Forcepage : Page
    {

        public Forcepage()
        {
            InitializeComponent();
        }

        public Forcepage(long patientid)
            : this()
        {
        }
    }
}
