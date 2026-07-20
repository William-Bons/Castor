using Castor.database;
using Castor.database.tables;
using Castor.gui.common;
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

namespace Castor.gui.commities
{
    /// <summary>
    /// Логика взаимодействия для CommitiesPage.xaml
    /// </summary>
    public partial class CommitiesPage : Page, INotifyPropertyChanged, IStartablePage
    {
        private Movebook mb;

        public event PropertyChangedEventHandler? PropertyChanged;

        public CommitiesPage()
        {
            InitializeComponent();
            LoadCommities();
            DataContext = this;
        }

        public IEnumerable<Commity>? Commities { get; set; }
        public Commity? SelectedCommity { get;  set; }

        public bool CanStart => true;

        public CommitiesPage(Movebook mb)
        {
            this.mb = mb;
            InitializeComponent();
            LoadCommities();
            DataContext = this;
        }

        private void LoadCommities()
        {
            using CastorContext castorContext = new CastorContext();
            if (mb != null)
            {
                Commities = castorContext.Commity
                    .Where(c => c.MovebookId == mb.Id)
                    .Include(c => c.Movebook)
                    .Include(c => c.Runs)
                    .ToList();
            }
            else
            {
                Commities = castorContext.Commity
                    .Include(c => c.Movebook)
                    .Include(c => c.Runs)
                    .ToList();
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Commities)));
        }
        private void OpenSelectedCommity(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
