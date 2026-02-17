using Castor.database;
using Castor.database.tables;
using Microsoft.EntityFrameworkCore;
using System.Windows;

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

        public Movebook Movebook { get; set; }

        private void SaveInDb(object sender, RoutedEventArgs e)
        {
            if (!Validate())
            {
                MessageBox.Show("Не все необходимые данные введены","ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (CastorContext castor = new CastorContext())
            {
                Movebook.Dateout = DateOnly.FromDateTime(DateInControl.SelectedDate.Value);
                FormattableString fstr = $@"UPDATE Movebooks set dsout={Movebook.Dsout}, dateout={Movebook.Dateout}, city={Movebook.City}, early={Movebook.Early}, unvoluntary={Movebook.Unvoluntary}, first={Movebook.First}, second={Movebook.Second}, closed={Movebook.Closed}, outto={Movebook.Outto}, deceased={Movebook.Deceased}
                  where Id={Movebook.Id}";
                castor.Database.ExecuteSql(fstr);
                castor.SaveChanges();
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Movebook.Dsout) && Movebook.Outto.HasValue && DateInControl.SelectedDate.HasValue;
        }
    }
}
