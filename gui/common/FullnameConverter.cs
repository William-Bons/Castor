using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Castor.gui.common
{
    class FullnameConverter : IValueConverter
    {
        // Converts DateTime (from ViewModel) to DateTime (for UI control's SelectedDate property, 
        // where the time part is often ignored or set to midnight by the control)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string fullName)
            {
                // Split the name by spaces
                string[] nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // Basic validation to ensure we have at least a first and last name
                if (nameParts.Length < 2)  return fullName; // Or handle as an error

                return $"{nameParts[0]} {char.ToUpper(nameParts[1][0])}.";

            }
            else
                return value;
        }

        // Converts DateTime (from UI control, e.g., DatePicker) to DateOnly (for ViewModel)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
