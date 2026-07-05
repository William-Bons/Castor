using BCrypt.Net;           // Библиотека для хеширования паролей (BCrypt.Net)
using Castor.database;
using Castor.database.tab_medis;
using Castor.database.tables; // Твой namespace с классом User
using Castor.gui.movebook;
using Castor.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Castor.gui.login
{
    public partial class UserCreateWindow : Window
    {
        private User? _editingUser;
        private bool _isEditMode;

        /// <summary>
        /// Конструктор для СОЗДАНИЯ нового пользователя
        /// </summary>
        public UserCreateWindow() : this( null) { }

        /// <summary>
        /// Конструктор для РЕДАКТИРОВАНИЯ существующего пользователя
        /// Если user != null — окно откроется в режиме редактирования
        /// </summary>
        public UserCreateWindow( User? user)
        {
            InitializeComponent();
            _editingUser = user;
            _isEditMode = (user != null);

            if (_isEditMode)
            {
                Title = "Редактирование пользователя";
                LoadUserData(user);
            }
            else
            {
                Title = "Создание пользователя";
            }
        }

        /// <summary>
        /// Заполняет поля окна данными пользователя
        /// </summary>
        private void LoadUserData(User user)
        {
            FullNameTextBox.Text = user.FullName;
            LoginTextBox.Text = user.Login;

            // Пароли НИКОГДА не показываем в открытом виде
            PasswordBox.Password = string.Empty;
            ConfirmPasswordBox.Password = string.Empty;

            // Выбираем роль в ComboBox
            SelectRoleInComboBox(user.Role);
        }

        /// <summary>
        /// Находит и выбирает нужную роль в ComboBox по значению строки
        /// Адаптируй логику под то, как ты заполняешь ComboBox (из БД или хардкод)
        /// </summary>
        private void SelectRoleInComboBox(string roleName)
        {
            foreach (var item in RoleComboBox.Items)
            {
                if (item is ComboBoxItem cbi && cbi.Content?.ToString() == roleName)
                {
                    RoleComboBox.SelectedItem = cbi;
                    return;
                }
            }

            // Если роль не найдена (редкий случай при рассинхронизации БД и UI), можно выбрать первый элемент или показать ошибку
            if (RoleComboBox.Items.Count > 0)
            {
                RoleComboBox.Items.Add(new ComboBoxItem() { Content=roleName});
                SelectRoleInComboBox(roleName);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var fullName = FullNameTextBox.Text.Trim();
            var login = LoginTextBox.Text.Trim();
            var roleItem = RoleComboBox.SelectedItem as ComboBoxItem;
            var password = PasswordBox.Password;
            var confirm = ConfirmPasswordBox.Password;

            // Базовые проверки
            if (string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Введите ФИО пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (roleItem == null || string.IsNullOrEmpty(roleItem.Content?.ToString()))
            {
                MessageBox.Show("Выберите роль пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string role = roleItem.Content.ToString();

            // Проверка совпадения паролей (только если пользователь начал вводить новый пароль)
            if (!string.IsNullOrEmpty(password))
            {
                if (password != confirm)
                {
                    MessageBox.Show("Пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            try
            {
                // Используем локальный контекст для изоляции транзакции сохранения
                using var localDb = new CastorContext();

                if (_isEditMode && _editingUser != null)
                {
                    // === РЕЖИМ РЕДАКТИРОВАНИЯ ===

                    // Проверяем, не занят ли этот логин кем-то другим (исключая текущего пользователя)
                    var existingUser = localDb.Users
                        .FirstOrDefault(u => u.Login == login && u.Id != _editingUser.Id);

                    if (existingUser != null)
                    {
                        MessageBox.Show($"Логин \"{login}\" уже занят пользователем \"{existingUser.FullName}\".",
                            "Ошибка уникальности", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    _editingUser.FullName = fullName;
                    _editingUser.Login = login;
                    _editingUser.Role = role;

                    // Обновляем пароль только если пользователь ввёл новый
                    if (!string.IsNullOrEmpty(password))
                    {
                        _editingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                    }

                    localDb.Users.Update(_editingUser);
                    localDb.SaveChanges();
                }
                else
                {
                    // === РЕЖИМ СОЗДАНИЯ ===

                    // Проверка уникальности логина при создании
                    var duplicate = localDb.Users.FirstOrDefault(u => u.Login == login);
                    if (duplicate != null)
                    {
                        MessageBox.Show($"Пользователь с логином \"{login}\" уже существует.",
                            "Ошибка уникальности", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string hash = string.IsNullOrEmpty(password)
                        ? BCrypt.Net.BCrypt.HashPassword("123456") // Дефолтный пароль, если не задан (лучше требовать ввод)
                        : BCrypt.Net.BCrypt.HashPassword(password);

                    var newUser = new User
                    {
                        FullName = fullName,
                        Login = login,
                        Role = role,
                        PasswordHash = hash,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    localDb.Users.Add(newUser);
                    localDb.SaveChanges();
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                var inner = ex.InnerException;

                // Попытка дать более понятное сообщение для ошибок БД (уникальность и т.д.)
                if (inner != null)
                {
                    errorMessage = inner.Message;
                    if (errorMessage.Contains("unique") || errorMessage.Contains("duplicate") || errorMessage.Contains("UNIQUE"))
                    {
                        MessageBox.Show("Такой логин уже существует в базе данных.", "Ошибка уникальности",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                MessageBox.Show($"Произошла ошибка при сохранении:\n{errorMessage}", "Ошибка базы данных",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // 50320-DOC, 50373-50380 MC
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            using MedisContext medisContext = new MedisContext();
            using CastorContext _db = new CastorContext();

            var docs = medisContext.docdep
                .Where(d => d.depid == Settings.Default.LastSelectedDepId)
                .Include(d => d.Position)
                .ToList();

            new SelectObjectFromEnumerable(docs, System.Windows.Controls.Primitives.PlacementMode.MousePoint,"text").Selected += (o) =>
            {
                if(o is docdep _doc)
                {
                    LoadUserData(new User()
                    {
                        FullName = _doc.text,
                        Login = LoginGenerator.GenerateUniqueLoginAsync(_doc.text).Result,
                        Role = _doc.Position.text

                    });
                }
            };
        }

        
    }
}
