using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Castor.Properties;
using Castor.test;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using static Castor.gui.common.IDialog;

namespace Castor.gui.dialogs
{
    /// <summary>
    /// Логика взаимодействия для CreateNewUser.xaml
    /// </summary>
    public partial class CreateNew : Window, IDialog
    {
        public event DialogOKHandler DialogOK;

        private CastorContext _castorContext;
        public CreateNew()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Movebook Movebook { get; private set; }

        private void SaveInDb(object sender, RoutedEventArgs e)
        {
            DialogOK?.Invoke();
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SelectPatientFromMedis(object sender, RoutedEventArgs e)
        {
            using (MedisContext medisContext = new MedisContext())
            {
                ICollection<dep>? depList = medisContext.dep
                    .Where(d => d.keyid == Settings.Default.LastSelectedDep)
                    .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                    .ThenInclude(v => v.Doctor)
                    .Include(d => d.Visits.Where(v => !v.dat1.HasValue))
                    .ThenInclude(v => v.Patient)
                    .ThenInclude(p => p.Diagnoses)
                    .ToList();
                TablePage tablePage = new TablePage(depList.First().Visits);
                
            }
        }
    }
}
