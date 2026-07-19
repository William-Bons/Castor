using Castor.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Castor.gui.movebook
{
    public class MoveFilter : INotifyPropertyChanged
    {
        public bool DatesSet { get; set; } = false;
        public DateTime Start { get; set; } = CalcStart();
        public DateTime End { get; set; } = CalcEnd();
        public bool IsHideDisordered 
        {
            get 
            {
                return Settings.Default.HideDisordered;
            }

            set
            {
                Settings.Default.HideDisordered = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        } 
        public bool IsHideClosedCards 
        { 
            get
            {
                return Settings.Default.HideClosedCards;
            }

            set
            {
                Settings.Default.HideClosedCards = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        } 
        
        public MoveFilter()
        {
        }

        public MoveFilter(int DaysBefore)
        {
            End = DateTime.Now;
            Start = DateTime.Now - TimeSpan.FromDays(DaysBefore);
        }

        public static DateTime CalcStart()
        {
            return DateTime.Parse($"{21}.{(DateTime.Today.Month > 1 ? DateTime.Today.Month - 1 : 12)}.{DateTime.Today.Year}");
        }

        public static DateTime CalcEnd()
        {
            return DateTime.Parse($"{20}.{DateTime.Today.Month}.{DateTime.Today.Year}");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

    public class MovefilterHeaderItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool _b && _b && parameter is string) ?
                $"\u2714 {parameter}" : parameter;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
