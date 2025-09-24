using Castor.database.tab_medis;
using Castor.gui.common;
using Castor.gui.dialogs;
using Castor.test;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.pages
{
    /// <summary>
    /// Логика взаимодействия для LoadPatientsFromMedis.xaml
    /// </summary>
    public partial class LoadPatientsFromMedis : Page, INotifyPropertyChanged, ISwithPage, IMainStatusBar
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event SwitchPageHandler? SwitchPage;
        public event PrintStatusMessageHandler? PrintStatusMessage;

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
                PrintStatusMessage?.Invoke($"Total count: {(depList?.Count > 0 ? LoadedData?.Visits?.Count : 0)}", "status");
            };
        }

        public dep? LoadedData
        {
            get => depList != null && depList?.Count > 0 ? depList?.First<dep>() : null;
        }


        private void LoadFromMedis()
        {
            using (MedisContext cc = new MedisContext())
            {
                depList = cc.dep
                    .Where(d => d.keyid == SelectUser.SelectedDep.keyid)
                    .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                    .ThenInclude(v => v.Doctor)
                    .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                    .ThenInclude(v => v.Patient)
                    .ThenInclude(p => p.Diagnoses)
                    .ToList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadedData)));
            }
        }

        /// <summary>
        /// Double click on grid`s cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PatientSelectedToPlan(object sender, MouseButtonEventArgs e)
        {
            visit _current = (visit)((DataGrid)sender).CurrentItem;
            //SwitchPage?.Invoke("Castor.gui.pages.MakeNewPlanning", _current);
            new TablePage(_current.Patient.Diagnoses);
            ;
        }

        private void CurrentDocdepFilter(object sender, System.Windows.RoutedEventArgs e)
        {
            using (MedisContext cc = new MedisContext())
            {
                
            }
        }
    }
}
