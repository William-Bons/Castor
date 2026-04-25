using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Castor.gui.common
{
    public interface ITableView;

    class TablePage
    {
        public DataTable Table { get; set; }

        public static void ShowPopup(object _obj)
        {
            if (_obj != null)
            {
                DataGrid dataGrid = new() { IsReadOnly = true, SelectedValuePath = "Value" };
                dataGrid.MouseDoubleClick += (a, b) =>
                {
                    if (dataGrid.SelectedValue is ITableView)
                        TablePage.ShowPopup(dataGrid.SelectedValue);
                };

                Popup popup = new Popup()
                {
                    Width = 400,
                    Height = 800,
                    Placement = PlacementMode.MousePoint,
                    Child = dataGrid,
                    IsOpen = true,
                    StaysOpen = false
                };
                Binding binding = TablePage.mkBinding(_obj);
                ((DataGrid)popup.Child).SetBinding(DataGrid.ItemsSourceProperty, binding);
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
                Binding binding = TablePage.mkBinding(_obj);
                ((DataGrid)w.Content).SetBinding(DataGrid.ItemsSourceProperty, binding);
                if (_obj is DataTable _t) w.Title = _t.TableName;
                w.Show();
                return w;
            }
            else
                return null;
        }

        public static void ShowDataset(DataSet set)
        {
            TabControl tab = new TabControl();
            foreach (DataTable table in set?.Tables)
            {
                TabItem item = new TabItem() { Content = new DataGrid() { CanUserAddRows = false }, Header = table.TableName };
                Binding binding = TablePage.mkBinding(table);
                ((DataGrid)item.Content).SetBinding(DataGrid.ItemsSourceProperty, binding);
                tab.Items.Add(item);
            }
            new Window() { Content = tab }.ShowDialog();
        }
        private static Binding mkBinding(object _obj)
        {
            TablePage page = new TablePage();
            Binding binding = new Binding();

            if (_obj is DataRowView _drw && _drw.Row is DataRow _arow)
            {
                binding.Source = page.MakeDataRow(_arow);
            }
            else if (_obj is DataRow _brow)
            {
                binding.Source = page.MakeDataRow(_brow);
            }
            else if (_obj is DataTable _dt)
            {
                binding.Source = _dt;
            }
            else if (_obj is ITableView)
            {
                binding.Source = page.MakeDbClass(_obj);
            }

            return binding;


        }

        private object MakeDbClass(object record)
        {
            try
            {
                Table = new DataTable();
                Table.Columns.Add("Field", typeof(string));
                Table.Columns.Add("Value", typeof(object));

                PropertyInfo[] propertyInfos = record.GetType().GetProperties();
                foreach (var field in propertyInfos)
                {
                    DataRow dataRow = Table.NewRow();
                    dataRow["Field"] = field.Name;
                    dataRow["Value"] = field.GetValue(record);
                    Table.Rows.Add(dataRow);
                }

                return Table;
            }
            catch
            {
                return null;
            }
        }

        private object MakeDataRow(DataRow _row)
        {
            try
            {
                Table = new DataTable();
                Table.Columns.Add("Field", typeof(string));
                Table.Columns.Add("Value", typeof(object));

                for (int i = 0; i < _row.Table.Columns.Count; i++)
                {
                    DataRow r = Table.NewRow();
                    r["Field"] = _row.Table.Columns[i].ColumnName;
                    r["Value"] = _row[i];
                    Table.Rows.Add(r);
                }
                return Table;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, exc.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
