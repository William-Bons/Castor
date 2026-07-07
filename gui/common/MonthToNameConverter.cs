using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Castor.gui.common
{
    public class MonthToNameConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Если значение null или не int — возвращаем пустую строку или заглушку
            if (value == null || !(value is int[] month) || month.Count()<2)
                return string.Empty;

            // culture.DateTimeFormat.GetMonthName(month) возвращает полное название месяца
            // month должен быть от 1 до 12
            var cultureInfo = culture ?? CultureInfo.CurrentCulture;
            return $"{cultureInfo.DateTimeFormat.GetMonthName(month[0])} - {cultureInfo.DateTimeFormat.GetMonthName(month[1])}";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Обратное преобразование (текст → число) в данном сценарии не нужно,
            // так как поле ввода остаётся числом. Возвращаем Binding.DoNothing.
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
