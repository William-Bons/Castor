using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace Castor.gui.common
{

    class Message
    {
        private static string _lastMessage = string.Empty;
        private static Message? _shown = null;
        
        private System.Timers.Timer timer;
        private Popup popup;

        private Message()
        {
            timer = new System.Timers.Timer(TimeSpan.FromSeconds(2));
            timer.Elapsed += (a, b) =>
            {
                popup.Dispatcher.Invoke(new Action(() =>
                {
                    popup.IsOpen = false;
                }), null);

                
            };
            timer.Start();
        }
        public static void ShowPopup(string _obj)
        {
            if(_shown != null)
            {
                _shown.timer.Stop();
                _shown.popup.IsOpen = false;
                _shown = null;
            }
            if (_obj != null)
            {
                TextBlock textBlock = new TextBlock()
                {
                    Text = _obj,
                    Padding = new Thickness(10),
                    FontFamily = new FontFamily("Courier New"),
                    FontSize = 14,
                    Background = Brushes.LightYellow
                };

                _shown = new Message();
                _shown.popup = new Popup()
                {
                    //Width = 1000,
                    //Height = 800,
                    Placement = PlacementMode.RelativePoint,
                    PlacementTarget = MainWindow.Instance,
                    Child = textBlock,
                    AllowsTransparency = true,
                    HorizontalOffset = 30,
                    VerticalOffset = 30,
                    IsOpen = true,
                    StaysOpen = false,
                };
            }
            else
            {
                Message.ShowPopup(_lastMessage);
            }
        }

        public static Window ShowWindow(object _obj)
        {
            if (_obj != null)
            {
                DataGrid dataGrid = new() { CanUserAddRows = false };
                Window w = new Window()
                {
                    Width = 400,
                    Height = 800,
                    Content = dataGrid
                };
                w.Show();
                return w;
            }
            else
                return null;
        }
    }
}
