using Castor.database.tables;
using Castor.gui.common;
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

namespace Castor.gui.medicalcomission
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ProtocolWindow : Window, IDialog
    {
        public ProtocolWindow()
        {
            InitializeComponent();

            var vm = new ProtocolViewModel(new MedicalCommissionProtocol());
            // Заполняем список ролей для ComboBox
            vm.RolesList = Enum.GetValues(typeof(CommissionMemberRole))
                .Cast<CommissionMemberRole>()
                .Select(r => r.ToString())
                .ToList();

            DataContext = vm;
        }
    }
}
