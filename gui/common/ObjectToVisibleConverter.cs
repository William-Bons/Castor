using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Castor.gui.common
{
    internal class ObjectToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int vi)
            {
                return vi > 0 ? Visibility.Visible :
                    Visibility.Hidden;
            }
            else if(value is long li)
                return li > 0 ? Visibility.Visible :
                    Visibility.Hidden;
            else if(value is IEnumerable<object> list)
                return list.Count() > 0 ? Visibility.Visible : Visibility.Hidden;
            else
                return Visibility.Hidden;
        }

        // Converts DateTime (from UI control, e.g., DatePicker) to DateOnly (for ViewModel)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
