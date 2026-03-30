using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Castor.gui.movebook
{
    /// <summary>
    /// </summary>
    public class SelectObjectFromEnumerable : DataGrid
    {
        public delegate void SelectPatientInVisitsEventHandler(object selected);
        public event SelectPatientInVisitsEventHandler Selected;
        private Popup _canvas;


        public SelectObjectFromEnumerable(string Title, IEnumerable<object> _RowSourceObject, PlacementMode mode=PlacementMode.MousePoint, params string[] columns)
            :this(mode,columns)
        {
            ItemsSource = _RowSourceObject;
            Grid gr = CreateGrid();
            TextBlock textBlock = new TextBlock() { Text = Title, Padding=new Thickness(5), FontFamily= new System.Windows.Media.FontFamily("Courier New"), FontSize=15, Foreground=Brushes.White};
            gr.Children.Add(textBlock);
            gr.Children.Add(this);
            _canvas.Child = gr;
            _canvas.IsOpen = true;
        }

        public SelectObjectFromEnumerable(IEnumerable<object> _RowSourceObject, PlacementMode mode=PlacementMode.MousePoint, params string[] columns)
            : this(mode,columns)
        {

            ItemsSource = _RowSourceObject;
            _canvas.Child = this;
            _canvas.IsOpen = true;
        }

        private SelectObjectFromEnumerable(PlacementMode mode, params string[] columns)
            :base()
        {
            /* Create columns */
            foreach (var column in columns)
            {
                DataGridTextColumn newColumn = new DataGridTextColumn();
                Binding bnd = new Binding(column);
                bnd.StringFormat = "{0:d}";

                newColumn.Header = column;
                newColumn.Binding = bnd;
                newColumn.MaxWidth = 500;

                Columns.Add(newColumn);
            }
            
            MouseDoubleClick += DataGrid_MouseDoubleClick;
            AutoGenerateColumns = false;
            IsReadOnly = true;
            Language = System.Windows.Markup.XmlLanguage.GetLanguage("RU");
            FontFamily = new System.Windows.Media.FontFamily("Courier New");
            FontSize = 15.0;

            _canvas = new Popup();
            _canvas.StaysOpen = false;
            _canvas.PlacementTarget = MainWindow.Instance;
            _canvas.Placement = mode;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _canvas.IsOpen = false;
            Selected?.Invoke(SelectedItem);
        }

        private Grid CreateGrid()
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(this, 1);
            return grid;
        }
    }
}
