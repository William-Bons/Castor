using Castor;
using Castor.gui.reports;
using Castor.gui.dialogs;
using Castor.gui.force;
using Castor.gui.movebook;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.login

{
    public partial class StartPage : Page
    {
        private readonly MainWindow mainWindow;

        public StartPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        // Переход на страницу "Движение пациентов"
        private void PatientsMovement_Click(object sender, MouseButtonEventArgs e)
        {
            // Вариант 1: Если используете Frame в главном окне
            if (mainWindow != null && sender is FrameworkElement fe && fe.Tag is string className)
            {
                // Предположим, что у вас есть Frame с именем CentralFrame
                mainWindow.ActivateByName(className);
            }

            
        }
    }
}