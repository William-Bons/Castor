using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables;
using Castor.gui.common;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static Castor.gui.common.IDialog;

namespace Castor.gui.movebook
{
    /// <summary>
    /// Логика взаимодействия для Disorder.xaml
    /// </summary>
    public partial class Disorder : Window, IDialog
    {
        public event DialogOKHandler? DialogOK;
        public Disorder(Movebook movebook)
        {
            Movebook = movebook;
            InitializeComponent();
            

            //PRELOAD
            try
            {
                using (MedisContext medis = new MedisContext())
                {
                    visit visit = medis.visit
                        .Where(v => v.keyid == movebook.Visitid)
                        .Include(v => v.Patient)
                        .ThenInclude(v => v.Diagnoses).First();

                    if (visit != null && visit.Patient.CurrentDs.diagid != null && visit.dat1.HasValue)
                    {
                        var DS = medis.diagnos
                                .Where(d => d.keyid == visit.Patient.CurrentDs.diagid)?.First();

                        var RD = medis.diagnos
                            .Where(d => d.keyid == DS.rootid)?.First();

                        Movebook.Dateout = DateOnly.FromDateTime(visit.dat1.Value);
                        Movebook.Dsout = RD.code;
                        Movebook.Outto = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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
            DialogOK?.Invoke();
            Close();
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

    public class DateOnlyConverter : IValueConverter
    {
        // Converts DateTime (from ViewModel) to DateTime (for UI control's SelectedDate property, 
        // where the time part is often ignored or set to midnight by the control)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnlyValue)
            {
                // Convert DateOnly to DateTime (arbitrary time, e.g., midnight)
                return new DateTime(dateOnlyValue.Year, dateOnlyValue.Month, dateOnlyValue.Day);
            }
            // Handle null values or other types
            return null;
        }

        // Converts DateTime (from UI control, e.g., DatePicker) to DateOnly (for ViewModel)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTimeValue)
            {
                // Use the static method to extract only the date part
                return DateOnly.FromDateTime(dateTimeValue);
            }
            // Handle null values or other types
            return null;
        }
    }
}
