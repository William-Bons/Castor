using System.Globalization;
using System.Windows.Data;

namespace Castor.gui.common
{
    class FssDateToColorConverter : IValueConverter
    {
        // Converts DateTime (from ViewModel) to DateTime (for UI control's SelectedDate property, 
        // where the time part is often ignored or set to midnight by the control)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly fssdate)
            {
                return fssdate == DateOnly.FromDateTime(DateTime.Today) ? 1 :
                    fssdate < DateOnly.FromDateTime(DateTime.Today) ? 2 :
                    0;
            }
            else
                return 0;
        }

        // Converts DateTime (from UI control, e.g., DatePicker) to DateOnly (for ViewModel)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
