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
using System.Windows.Shapes;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для Disorder.xaml
    /// </summary>
    public partial class Disorder : Window
    {
        public Disorder(Movebook movebook)
        {
            Movebook = movebook;
            InitializeComponent();
            DataContext = this;
        }

        public Movebook Movebook {  get; set; }

        private void SaveInDb(object sender, RoutedEventArgs e)
        {
            using (CastorContext castor = new CastorContext())
            {
                Movebook.Dateout = DateOnly.FromDateTime(DateInControl.SelectedDate.Value);
                FormattableString fstr = $@"UPDATE Movebooks set dsout={Movebook.Dsout}, dateout={Movebook.Dateout}, city={Movebook.City}, early={Movebook.Early}, unvoluntary={Movebook.Unvoluntary}, first={Movebook.First}, second={Movebook.Second}, closed={Movebook.Closed}, outto={Movebook.Outto}
                  where Id={Movebook.Id}";
                castor.Database.ExecuteSql(fstr);
                castor.SaveChanges();
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
