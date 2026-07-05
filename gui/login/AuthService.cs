using Castor.database;
using Castor.database.tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Castor.gui.login
{
    public class AuthService
    {

        public AuthService()
        {
        }

        public bool ValidateCredentials(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                using var context = new CastorContext();

                // Ищем ТОЛЬКО по логину
                var user = context.Users
                    .FirstOrDefault(u => u.Login == login);

                if (user == null)
                {
                    App.LogWarning("Auth", $"Попытка входа с неизвестным логином: {login}");
                    return false;
                }

                // BCrypt.Verify сам извлекает соль из хеша и делает проверку
                bool isMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

                if (!isMatch)
                {
                    App.LogWarning("Auth", $"Неверный пароль для пользователя: {login}");
                }

                return isMatch;
            }
            catch (Exception ex)
            {
                App.LogError("AuthService", ex);
                return false;
            }
        }


        /// <summary>
        /// Создание тестового пользователя (только для разработки!)
        /// В продакшене используй отдельный экран «Регистрация» или API.
        /// </summary>
        public void CreateUser(string login, string plainPassword)
        {
            using var context = new CastorContext();

            // Проверяем, нет ли уже такого логина
            if (context.Users.Any(u => u.Login == login))
                throw new InvalidOperationException($"Пользователь с логином '{login}' уже существует.");

            string hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            var newUser = new User
            {
                Login = login,
                PasswordHash = hash // строка сразу готова к BCrypt.Verify
            };

            context.Users.Add(newUser);
            context.SaveChanges();
        }

    }
}
