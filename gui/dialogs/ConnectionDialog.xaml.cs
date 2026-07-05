using Castor.Properties;
using Npgsql;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; // Обязательно для Brushes, SolidColorBrush

namespace Castor.gui.dialogs
{
    public partial class ConnectionDialog : Window
    {
        public string ConnectionString { get; private set; }

        public ConnectionDialog() => InitializeComponent();

        private void SetStatus(string text, bool isError = false)
        {
            StatusLabel.Text = text;

            if (isError)
            {
                // Пытаемся взять кисть ошибки из твоих ресурсов (если вдруг добавишь Brush.Error)
                if (TryGetBrush("Brush.Error", out var errorBrush))
                    StatusLabel.Foreground = errorBrush;
                else
                    // Если нет, используем спокойный тёмно-красный, который не конфликтует с палитрой
                    StatusLabel.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));

                StatusLabel.FontWeight = FontWeights.SemiBold;
            }
            else
            {
                // Твоя основная кисть текста
                if (TryGetBrush("Brush.TextMain", out var textBrush))
                    StatusLabel.Foreground = textBrush;
                else
                    StatusLabel.Foreground = Brushes.DimGray;

                StatusLabel.FontWeight = FontWeights.Normal;
            }
        }

        // Безопасное получение кисти из ресурсов CastorSoft
        private bool TryGetBrush(string key, out Brush brush)
        {
            brush = null;
            if (Application.Current?.Resources.Contains(key) == true)
            {
                var resource = Application.Current.Resources[key];
                if (resource is Brush b)
                {
                    brush = b;
                    return true;
                }
            }
            return false;
        }

        private string BuildConnectionString()
        {
            var host = HostBox.Text.Trim();
            var port = PortBox.Text.Trim();
            var database = DatabaseBox.Text.Trim();
            var user = UserBox.Text.Trim();
            var password = PasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(database) ||
                string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                SetStatus("Заполните все обязательные поля.", true);
                return null;
            }

            if (!int.TryParse(port, out int portNum) || portNum <= 0 || portNum > 65535)
            {
                SetStatus("Некорректный номер порта.", true);
                return null;
            }

            return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            SetStatus("Проверка соединения...");
            var connString = BuildConnectionString();
            if (connString == null) return;

            try
            {
                using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                SetStatus("Соединение успешно!", false);

                // Если позже добавишь Brush.Success, можно раскомментировать:
                // if (TryGetBrush("Brush.Success", out var successBrush)) StatusLabel.Foreground = successBrush;
            }
            catch (Exception ex)
            {
                var msg = ex.Message.Split('\n')[0]; // Оставляем только первую строку для чистоты
                SetStatus($"Ошибка: {msg}", true);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var connString = BuildConnectionString();
            if (connString == null) return;

            Settings.Default.postgreeConnection = Encrypt(connString);
            Settings.Default.Save();
            
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public string Encrypt(string plainText, string password = null)
        {
            var data = Encoding.Default.GetBytes(plainText);
            var pwd = !string.IsNullOrEmpty(password) ? Encoding.Default.GetBytes(password) : Array.Empty<byte>();
            var cipher = ProtectedData.Protect(data, pwd, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(cipher);
        }
    }
}
