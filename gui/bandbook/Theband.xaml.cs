using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.bandbook
{
    /// <summary>
    /// Логика взаимодействия для Theband.xaml
    /// </summary>
    public partial class Theband : Page, INotifyPropertyChanged, IRefresh, IStartablePage, IConsoleMessage
    {
        private CastorContext context;
        private bool need_save = false;

        public Theband()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ICollection<Bandbook> QuartalData { get; private set; } = new List<Bandbook>();
        public Quartal Quartal { get; private set; } = new Quartal() { start = DateTime.Parse("2026-01-01"), end = DateTime.Now };
        public double Persentage { get; private set; } = 0.69;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public event common.RefreshEventHandler RefreshNotify;
        public event ConsoleMessageHandler ConsoleMessage;


        private async Task Load(DatePeriod Period)
        {
            if (context != null)  context.Dispose();
            context = new CastorContext();
            QuartalData = context.Bandbooks
                .ToList();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuartalData)));
            
        }

        public async  void Refresh()
        {
            await Task.Run(() => Load(new DatePeriod()));
            
        }

        private void LoadQuartal(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MedisContext castor = new MedisContext())
                {
                    var moving = castor.dep
                            .Where(d => d.keyid == Settings.Default.LastSelectedDep)
                            .Include(d => d.Visits.Where(v => v.dat >= Quartal.start.ToUniversalTime() && v.dat <= Quartal.end.ToUniversalTime()))
                            .ThenInclude(v => v.Patient)
                            .ThenInclude(p => p.Diagnoses)
                            .First().Visits.ToList();

                    double comma = moving.Count() / ((double)moving.Count() * Persentage);
                    double teck = 0;

                    List<int> indexes = new List<int>();
                    for (int idx = 0; idx < moving.Count(); idx += (int)Math.Round(comma))
                    {
                        indexes.Add(idx);
                        teck += comma;
                        idx = (int)Math.Round(teck);
                    }

                    QuartalData.Clear();
                    foreach (var index in indexes)
                    {
                        Bandbook bandbook = new Bandbook();
                        bandbook.Movebookid = moving.ElementAt(index).keyid;
                        bandbook.point01 = moving.ElementAt(index).Patient.fullname;
                        bandbook.point02 = moving.ElementAt(index).Patient.birthdate.ToString();
                        QuartalData.Add(bandbook);
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message.ToString());
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuartalData)));
        }

        private void ShowSocial(object sender, RoutedEventArgs e)
        {
            Framer.Children.Clear();
            Framer.Children.Add(new SocialPage());
        }
    }
}
