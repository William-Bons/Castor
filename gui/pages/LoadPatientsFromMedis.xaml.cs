using Castor.database.tab_medis;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.pages
{
    /// <summary>
    /// Логика взаимодействия для LoadPatientsFromMedis.xaml
    /// </summary>
    public partial class LoadPatientsFromMedis : Page, INotifyPropertyChanged, ISwithPage
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event SwitchPageHandler SwitchPage;

        private ICollection<dep>? depList;
        public LoadPatientsFromMedis()
        {
            InitializeComponent();
            DataContext = this;
            Cursor = Cursors.Wait;

            Loaded += async (o, e) =>
            {
                await Task.Run(() => LoadFromMedis());
                Cursor = Cursors.Arrow;
            };
        }

        public object LoadedData
        {
            get => depList != null && depList?.Count > 0 ? depList?.First<dep>() : null;
        }


        private void LoadFromMedis()
        {
            using (MedisContext cc = new MedisContext())
            {
                depList = cc.dep
                    .Where(d => d.keyid == 2704)
                    .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                    .ThenInclude(v => v.Doctor)
                    .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                    .ThenInclude(v => v.Patient)
                    .ToList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadedData)));
            }
        }

        private void PatientSelectedToPlan(object sender, MouseButtonEventArgs e)
        {
            visit _current = (visit)((DataGrid)sender).CurrentItem;
            SwitchPage?.Invoke("Castor.gui.pages.MakeNewPlanning", _current);
        }
    }
}
