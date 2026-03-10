using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows.Controls;

namespace Castor.gui.pages
{
    public partial class ForceControl : UserControl, INotifyPropertyChanged, IRefresh
    {
        public ForceControl()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += (a, b) =>
            {
                /*Task.Run(() =>*/
                Calculate();//);
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
                    FssList = castor.Movebooks.Where(m => m.Forced.HasValue)
                        .Include(m => m.ForceControl)
                        .ToList();
                        
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FssList)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
