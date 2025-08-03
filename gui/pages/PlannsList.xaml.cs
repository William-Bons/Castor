using Castor.database;
using Castor.database.tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Castor.gui.pages
{
    /// <summary>
    /// Логика взаимодействия для PlannsList.xaml
    /// </summary>
    public partial class PlannsList : Page, INotifyPropertyChanged
    {
        public PlannsList()
        {
            InitializeComponent();
            DataContext = this;

            Cursor = Cursors.Wait;
            Loaded += async (o,e) =>
            {
                await Task.Run(() => LoadPlanns());
                Cursor = Cursors.Arrow;
            };
        }

        public ICollection<planning> Plannings { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void LoadPlanns()
        {
            using (CastorContext cc = new CastorContext(CastorContext.ContextVariant.SQLITE))
            {
                Plannings = cc.Plannings
                    .Include(d => d.Dictionary)
                    .ToList();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Plannings)));
            }
        }

        private void PlanSelected(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
