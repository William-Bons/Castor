using System.Windows;
using System.Windows.Controls;

namespace Castor
{
    /// <summary>
    /// Логика взаимодействия для Kurwa.xaml
    /// </summary>
    public partial class Kurwa : Page
    {
        public Kurwa()
        {
#if DEBUG
            DebugMsg = Visibility.Visible;
#endif
            InitializeComponent();
            DataContext = this;
        }

        public Visibility DebugMsg { get; set; } = Visibility.Collapsed;
    }
}
