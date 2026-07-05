using Castor.database;
using Castor.database.tables;
using Castor.gui.login;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Castor.gui.login
{
    public partial class UsersListPage : Page
    {
        private readonly CastorContext _db;

        public UsersListPage()
        {
            InitializeComponent();
            _db = new CastorContext();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = _db.Users.ToList();
            UsersDataGrid.ItemsSource = users;
        }

        // Добавление нового пользователя
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserCreateWindow();
            var result = dialog.ShowDialog();
            if (result == true)
            {
                LoadUsers();
                MessageBox.Show("Пользователь добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Редактирование по кнопке (если вдруг понадобится отдельно)
        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            OpenEditWindow();
        }

        // Открытие окна редактирования
        private void OpenEditWindow()
        {
            var selected = UsersDataGrid.SelectedItem as User;
            if (selected == null) return;

            var dialog = new UserCreateWindow( selected);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                LoadUsers();
            }
        }

        // Двойной клик по строке → редактирование
        private void UsersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Защита от клика по пустым местам и заголовкам
            var row = (sender as DataGrid)?.GetRowFromPoint(e.GetPosition(sender as DataGrid));
            if (row == null || row.Item == null) return;

            var user = row.Item as User;
            if (user == null) return;

            OpenEditWindow();
        }

        // Переключение статуса по кнопке в колонке
        private void ToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                try
                {
                    user.IsActive = !user.IsActive;
                    _db.SaveChanges();

                    // Обновляем весь список, чтобы пересчитались статусы и надписи на кнопках
                    LoadUsers();

                    string action = user.IsActive ? "включён" : "отключён";
                    MessageBox.Show($"Пользователь «{user.FullName}» {action}.", "Статус изменён", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось изменить статус: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadUsers(); // Откат отображения к актуальному состоянию
                }
            }
        }

        // Обработка выбора строки (для кнопок внизу, если они будут видны)
        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = UsersDataGrid.SelectedItem as User;
            bool hasSelection = selected != null;
        }

    }

    public static class DataGridExtensions
    {
        public static DataGridRow? GetRowFromPoint(this DataGrid dataGrid, Point point)
        {
            var element = dataGrid.InputHitTest(point) as DependencyObject;
            while (element != null && !(element is DataGridRow))
            {
                element = VisualTreeHelper.GetParent(element);
            }
            return element as DataGridRow;
        }
    }

    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (parameter is string s)
                {
                    var parts = s.Split('|');
                    return b ? parts.Length > 0 ? parts[0] : "Да" : parts.Length > 1 ? parts[1] : "Нет";
                }
                return b ? "Да" : "Нет";
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool b)
                return DependencyProperty.UnsetValue;

            string? param = parameter as string;
            if (string.IsNullOrEmpty(param))
                return b ? Colors.Green : Colors.Gray;

            var parts = param.Split('|', StringSplitOptions.None);
            string colorNameTrue = parts.Length > 0 ? parts[0].Trim() : "Green";
            string colorNameFalse = parts.Length > 1 ? parts[1].Trim() : "Gray";

            Color colorTrue = ParseColor(colorNameTrue) ?? Colors.Green;
            Color colorFalse = ParseColor(colorNameFalse) ?? Colors.Gray;

            return b ? colorTrue : colorFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException("BoolToColorConverter не поддерживает ConvertBack");

        private static Color? ParseColor(string name)
        {
            // 1. Сначала пробуем известное имя цвета из Colors.Red, Colors.Green и т.д.
            var prop = typeof(Colors).GetProperty(name, BindingFlags.Public | BindingFlags.Static);
            if (prop?.PropertyType == typeof(Color))
                return (Color)prop.GetValue(null)!;

            // 2. Если это HEX (#RRGGBB, #AARRGGBB и т.п.) — используем ColorConverter
            if (name.StartsWith("#", StringComparison.Ordinal))
            {
                var converter = new ColorConverter();
                if (converter.CanConvertFrom(typeof(string)) && converter.ConvertFrom(name) is Color c)
                    return c;
            }

            return null;
        }
    }
}
