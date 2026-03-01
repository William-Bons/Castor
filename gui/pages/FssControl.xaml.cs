using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
using System.ComponentModel;
using System.Windows.Controls;

namespace Castor.gui.pages
{
    /// <summary>
    /// Логика взаимодействия для FssControl.xaml
    /// </summary>
    public partial class FssControl : UserControl, INotifyPropertyChanged, IRefresh
    {
        public FssControl()
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
                    FssList = castor.Movebooks.AsEnumerable().Where(x => x.FssDay>1).ToList();
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
