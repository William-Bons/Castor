using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows.Controls;

namespace Castor.gui.pages
{
    public partial class UnvlWidget : UserControl, INotifyPropertyChanged, IRefresh
    {
        public UnvlWidget()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += (a, b) =>
            {
                /*Task.Run(() =>*/
                Calculate();//);
            };
        }

        public ICollection<Movebook> UnvlList { get; set; }

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
                    UnvlList = castor.Movebooks
                        .Where(m => m.Unvoluntaryid.HasValue)
                        .Include(m => m.UnvoluntaryControl)
                        .Where(f => f.UnvoluntaryControl.Nextvk.Value <= DateOnly.FromDateTime(DateTime.Today))
                        .ToList();
                        
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnvlList)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Movebook? movebook = (sender as DataGrid)?.SelectedItem as Movebook;
                Unvoluntary? unvl = movebook?.UnvoluntaryControl;
                UnvoluntaryControl control = new UnvoluntaryControl(unvl);
                if (control.ShowDialog().Value)
                {
                    using (CastorContext castor = new CastorContext())
                    {
                        castor.Unvoluntaries.Update(unvl);
                        castor.SaveChanges();
                    }
                }
            }
            catch { }
        }
    }
}
