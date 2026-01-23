using Castor.database;
using Castor.database.tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    public partial class Thebook : Page
    {
        private CastorContext context;

        public Thebook(CastorContext castorContext)
        {
            InitializeComponent();
            context = castorContext;

            
        }
        public Movebook LoadedData { get; private set; }

        private async Task Load()
        {
            await Task.Run(() =>
            {
                LoadedData = context.Movebooks.ToListAsync();
            });
        }
    }
}
