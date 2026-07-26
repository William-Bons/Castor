using System.Windows;
using System.Windows.Controls;

namespace Castor.gui
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        public void UpdateProgress(string status, int progress)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = status;
                ProgressBar.Value = progress;
            });
        }

        public void SetIndeterminate(bool isIndeterminate)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.IsIndeterminate = isIndeterminate;
            });
        }
    }
}