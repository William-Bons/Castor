using Castor.database.tables;
using Castor.Properties;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Castor.gui.common
{

    class Message
    {
        private static string _lastMessage = string.Empty;
        private static List<Message?> Messages = new List<Message?>();

        private System.Timers.Timer timer;
        private Popup popup = new Popup();

        private Message()
        {
            timer = new System.Timers.Timer(TimeSpan.FromSeconds(Settings.Default.MessageShowLatency));
            timer.Elapsed += (a, b) =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    popup.IsOpen = false;
                    Messages.Remove(this);
                }), null);

            };
            timer.Start();
        }
        public static void ShowPopup(string _obj)
        {
            if (_obj != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Message _shown = new Message();
                    Messages.Add(_shown);

                    TextBlock textBlock = new TextBlock()
                    {
                        Text = _obj,
                        Padding = new Thickness(10),
                        FontFamily = new FontFamily("Courier New"),
                        FontSize = 15,
                        Background = Brushes.LightYellow
                    };


                    _shown.popup = new Popup()
                    {
                        //Width = 1000,
                        //Height = 800,
                        Placement = PlacementMode.RelativePoint,
                        PlacementTarget = MainWindow.Instance,
                        Child = textBlock,
                        AllowsTransparency = true,
                        HorizontalOffset = 30,
                        VerticalOffset = 30 + Messages.Count * 100,
                        IsOpen = true,
                        StaysOpen = true,
                    };
                    _shown.popup.MouseDown += (a, b) =>
                    {
                        Clipboard.SetText(_obj);
                    };
                }), null);
            }
            else
            {
                Message.ShowPopup(_lastMessage);
            }
        }
    }
}
