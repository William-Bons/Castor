using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;

namespace Castor.gui.pages
{
    public partial class ForceWidget : UserControl, INotifyPropertyChanged, IRefresh
    {
        public ForceWidget()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += (a, b) =>
            {
                /*Task.Run(() =>*/
                Calculate();//);
            };
        }

        public ICollection<Movebook> ForceList { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event common.RefreshEventHandler RefreshNotify;

        public void Refresh()
        {
            Task.Run(async () => Calculate());
        }

        private async Task Calculate()
        {
            try
            {
                using (CastorContext castor = new CastorContext())
                {
                    ForceList = castor.Movebooks
                        .Where(m => m.Forceds.Any())
                        .Include(m => m.Forceds)
                        //.Where(f => f.ForceControl.Nextvk.Month <= DateOnly.FromDateTime(DateTime.Today).Month)
                        .ToList();
                        
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceList)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //try
            //{
            //    PatientRecord? movebook = (sender as DataGrid)?.SelectedItem as PatientRecord;
            //    Forced? forced = movebook?.ForceControl;
            //    ForceControl control = new ForceControl(forced);
            //    if (control.ShowDialog().Value)
            //    {
            //        using (CastorContext castor = new CastorContext())
            //        {
            //            castor.Forced.Update(forced);
            //            castor.SaveChanges();
            //        }
            //    }
            //}
            //catch { }
        }
    }
}
