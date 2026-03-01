using System.Globalization;
using System.Windows.Data;

namespace Castor.gui.common
{
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
