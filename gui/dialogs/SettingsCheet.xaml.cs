using Castor.gui.common;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для SettingsCheet.xaml
    /// </summary>
    public partial class SettingsCheet : Window, IDialog
    {
        public SettingsCheet()
        {
            MakeATable();

            InitializeComponent();
            DataContext = this;
        }

        public DataTable DataRows { get; set; }

        private void MakeATable()
        {

            DataRows = new DataTable();
            DataRows.Columns.Add("Name", typeof(string));
            DataRows.Columns.Add("Type", typeof(Type));
            DataRows.Columns.Add("Value", typeof(object));

            foreach (SettingsProperty v0 in Properties.Settings.Default.Properties)
            {
                DataRow dr = DataRows.NewRow();
                dr["Name"] = v0.Name;
                dr["Type"] = v0.PropertyType;
                dr["Value"] = Properties.Settings.Default[v0.Name];
                DataRows.Rows.Add(dr);
            }
        }

        private void AssemblyValues()
        {
            foreach (DataRow dr in DataRows.Rows)
            {
                if (dr["Type"] is Type ty && ty == typeof(string))
                {
                    Properties.Settings.Default[dr["Name"]?.ToString()] = dr["Value"];
                }
                else if (dr["Type"] is Type tp && tp == typeof(bool))
                {
                    Properties.Settings.Default[dr["Name"]?.ToString()] =
                        dr["Value"]?.ToString() == "True" ? true :
                        dr["Value"]?.ToString() == "False" ? false : false;
                }
                else if (dr["Type"] is Type ti && ti == typeof(int) &&
                    int.TryParse(dr["Value"]?.ToString(), out int _ival))
                {
                    Properties.Settings.Default[dr["Name"]?.ToString()] = _ival;
                }
            }
        }

        private void Save(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                AssemblyValues();
                Properties.Settings.Default.Save();
                Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
