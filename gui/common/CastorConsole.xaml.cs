using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Castor.gui
{
    /// <summary>
    /// Логика взаимодействия для CastorConsole.xaml
    /// </summary>
    public partial class CastorConsole : UserControl
    {
        ConsoleContent dc = new ConsoleContent();

        public CastorConsole()
        {
            InitializeComponent();
            DataContext = dc;
            Loaded += CastorConsole_Loaded;
        }

        public void Print(string  message)
        {
            dc.ConsoleOutput.Add(message);
        }

        private void CastorConsole_Loaded(object sender, RoutedEventArgs e)
        {
            InputBlock.KeyDown += InputBlock_KeyDown;
            InputBlock.Focus();
        }

        void InputBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dc.ConsoleInput = InputBlock.Text;
                dc.RunCommand();
                InputBlock.Focus();
                Scroller.ScrollToBottom();
            }
        }
    }

    public class ConsoleContent : INotifyPropertyChanged
    {
        string consoleInput = string.Empty;
        ObservableCollection<string> consoleOutput = new ObservableCollection<string>() { "Console Emulation Sample..." };
        public event PropertyChangedEventHandler PropertyChanged;

        public string ConsoleInput
        {
            get => consoleInput;
            set
            {
                consoleInput = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConsoleInput"));
            }
        }

        public ObservableCollection<string> ConsoleOutput
        {
            get => consoleOutput;
            set
            {
                consoleOutput = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConsoleOutput"));
            }
        }

        public void RunCommand()
        {
            ConsoleOutput.Add(ConsoleInput);
            //TODO do your stuff here.              Rise custom event with text command
            ConsoleInput = String.Empty;
        }


        
        
    }
}
