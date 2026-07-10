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
    /// <summary>
    /// Логика взаимодействия для FssWidget.xaml
    /// </summary>
    public partial class FssWidget : UserControl, INotifyPropertyChanged, IRefresh
    {
        public FssWidget()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += (a, b) =>
            {
                Task.Run(async () => await Calculate());
            };
        }

        public List<Movebook> FssList { get; set; } = new();

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
                    //FssList = castor.Movebooks
                    //    .Where(x => x.Fssid.HasValue)
                    //    .Include(x => x.FssControl)
                    //    .Where(f => f.FssControl.Nextvk <= DateOnly.FromDateTime(DateTime.Today) && !f.FssControl.End.HasValue)
                    //    .ToList();
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FssList)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Movebook? movebook = (sender as DataGrid)?.SelectedItem as Movebook;
                Fss? fss = new(); // movebook?.FssControl; //todo
                FssControl control = new FssControl(fss);
                if (control.ShowDialog().Value)
                {
                    using (CastorContext castor = new CastorContext())
                    {
                        castor.Fss.Update(fss);
                        castor.SaveChanges();
                    }
                }
            }
            catch { }
        }
    }


}
