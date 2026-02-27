using Castor.database;
using Castor.gui.common;
using System.ComponentModel;
using System.Windows.Controls;

namespace Castor.gui.pages
{
    /// <summary>
    /// Логика взаимодействия для Weekmove.xaml
    /// </summary>
    public partial class Weekmove : UserControl, INotifyPropertyChanged, IRefresh
    {
        public Weekmove()
        {
            InitializeComponent();

            End = DateTime.Now;
            Start = DateTime.Now.AddDays(-7);

            Loaded += (s, a) =>
            {
                Task.Run(() => Calculate(s,a));
            };

            DataContext = this;
        }

        public int Entered { get; set; }
        public int Exited { get; set; }
        public int Closed { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event common.RefreshEventHandler RefreshNotify;

        public void Refresh()
        {
            Task.Run(() => Calculate(null, null));
        }

        private async void Calculate(object s, EventArgs a)
        {
            using (CastorContext castor = new CastorContext())
            {
                Entered = castor.Movebooks.Where(x => x.Datein >= DateOnly.FromDateTime(Start) && x.Datein <= DateOnly.FromDateTime(End)).Count();
                Exited = castor.Movebooks.Where(x => x.Dateout >= DateOnly.FromDateTime(Start) && x.Dateout <= DateOnly.FromDateTime(End)).Count();
                Closed = castor.Movebooks.Where(x => x.Dateout >= DateOnly.FromDateTime(Start) && x.Dateout <= DateOnly.FromDateTime(End) && x.Closed==true).Count();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Entered)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Exited)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Closed)));
            }
        }
    }
}
