using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Castor.gui.movebook
{
    /// <summary>
    /// </summary>
    public class SelectObjectFromEnumerable : DataGrid
    {
        public delegate void SelectPatientInVisitsEventHandler(object selected);
        public event SelectPatientInVisitsEventHandler Selected;
        private Popup _canvas;
        public SelectObjectFromEnumerable(IEnumerable<object> _RowSourceObject, params string[] columns)
            : base()
        {
            /* Create columns */
            foreach (var column in columns)
            {
                DataGridTextColumn newColumn = new DataGridTextColumn();
                newColumn.Header = column;
                newColumn.Binding = new Binding(column);
                Columns.Add(newColumn);
            }

            ItemsSource = _RowSourceObject;
            MouseDoubleClick += DataGrid_MouseDoubleClick;
            AutoGenerateColumns = false;
            IsReadOnly = true;
            Language = System.Windows.Markup.XmlLanguage.GetLanguage("RU");
            FontFamily = new System.Windows.Media.FontFamily("Courier New");
            FontSize = 15.0;

            _canvas = new Popup();
            _canvas.Child = this;
            _canvas.StaysOpen = false;
            _canvas.Placement = PlacementMode.MousePoint;
            _canvas.IsOpen = true;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _canvas.IsOpen = false;
            Selected?.Invoke(SelectedItem);
        }
    }
}
