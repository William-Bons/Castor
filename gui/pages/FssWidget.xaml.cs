using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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

        public ICollection<Movebook> FssList { get; set; }

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
                    FssList = castor.Movebooks.Where(x => x.Fssid.HasValue)
                        .Include(x => x.FssControl)
                        .ToList();
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FssList)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Movebook? fssline = FssGrid.SelectedItem as Movebook;
            //Popup popup = new Popup();

            //DatePicker datePicker = new DatePicker();
            //DateTime nd = fssline.Fssid.Value.ToDateTime(TimeOnly.MinValue) + TimeSpan.FromDays(15);
            //nd = nd - TimeSpan.FromDays(
            //    nd.DayOfWeek == DayOfWeek.Sunday ? 2 :
            //    nd.DayOfWeek == DayOfWeek.Saturday ? 1 :
            //    0
            //    );
            //datePicker.SelectedDate = nd;
            //datePicker.Margin = new Thickness(10);
            //datePicker.SelectedDateChanged += (a, b) =>
            //{
            //    fssline.Fssid = b.AddedItems.Count>0 ? b.AddedItems[0] as DateOnly? : null;    
            //    popup.IsOpen = false;
            //};

            
            //popup.Child = datePicker;
            //popup.StaysOpen = false;
            //popup.Placement = PlacementMode.Mouse;
            //popup.IsOpen = true;
        }
    }

    
}
