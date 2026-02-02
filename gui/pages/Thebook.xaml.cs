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
    /// Логика взаимодействия для Thebook.xaml
    /// </summary>
    public partial class Thebook : Page, INotifyPropertyChanged
    {
        private CastorContext context;

        public Thebook(CastorContext castorContext)
        {
            InitializeComponent();
            context = castorContext;
            DataContext = this;

            Loaded += async (a, b) =>
            {
                await Task.Run(() => Load());
            };
            
        }
        public ICollection<Movebook> LoadedData { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async Task Load()
        {

            LoadedData = context.Movebooks.ToList<Movebook>();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadedData)));
        }
    }
}
