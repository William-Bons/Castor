using Castor.database;
using Castor.database.tables;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Castor.gui.login
{
    public partial class LoginWindow : Window
    {


        public LoginWindow()
        {
            InitializeComponent();
            // Если хочешь заполнить комбобокс списком логинов из БД - 
            // ЭТО НУЖНО ДЕЛАТЬ В App.xaml.CS ПЕРЕД ПОКАЗОМ ОКНА, 
            // либо оставить пустым и вводить логин вручную.
            // Для чистоты архитектуры лучше вводить логин вручную в TextBox, а не выбирать из ComboBox.

            LoadLoginsAsync();
        }

        private async void LoadLoginsAsync()
        {
            try
            {
                // Асинхронно тянем только логины (не грузим хеши паролей зря)
                using var context = new CastorContext();
                var logins = await context.Users
                    .ToListAsync();

                LoginComboBox.ItemsSource = logins;

                if (logins.Any())
                {
                    LoginComboBox.SelectedValue = Settings.Default.LastConnectedUserId;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки логинов: {ex.Message}");
                // Тут можно показать MessageBox или лог, но не ломать окно входа
            }
        }

        // Методы для получения данных наружу (как у тебя было раньше)
        public string GetLogin() => (LoginComboBox.SelectedItem as User)?.Login ?? string.Empty;


        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginComboBox.SelectedItem?.ToString() ?? string.Empty;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Мы НЕ проверяем пароль здесь. Мы просто говорим App: "Пользователь ввёл вот это".
            // Результат проверки будет обработан в App.xaml.cs
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            PasswordBox.Clear(); // Сброс пароля
            DialogResult = false; // Отмена
            Close();
        }

        private void LoginComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если пользователь что-то выбрал — ставим фокус на пароль
            if (LoginComboBox.SelectedItem != null)
            {
                Settings.Default.LastConnectedUserId = (LoginComboBox.SelectedItem as User)?.Id ?? 0;
                PasswordBox.Focus();
            }
        }

    }
}
