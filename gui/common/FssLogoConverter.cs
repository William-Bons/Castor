using Castor.database.tables;
using System.Globalization;
using System.Windows.Data;

namespace Castor.gui.common
{
    public class FssLogoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Fss _fss)
            {
                return _fss.End > DateOnly.MinValue ?
                    new Uri("pack://application:,,,/img/fsslogo-2.png") :
                    new Uri("pack://application:,,,/img/fsslogo.png");
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
