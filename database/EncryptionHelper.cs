using System.Security.Cryptography;
using System.Text;

namespace Castor.database
{

    public class EncryptionHelper
    {
        // Фиксированная кодировка для совместимости
        private static readonly Encoding _encoding = Encoding.UTF8;

        /// <summary>
        /// Шифрует текст с использованием DPAPI
        /// </summary>
        /// <param name="plainText">Текст для шифрования</param>
        /// <param name="password">Дополнительный пароль (опционально)</param>
        /// <param name="scope">Уровень защиты (User/Machine)</param>
        /// <returns>Base64 зашифрованная строка</returns>
        public string Encrypt(
            string plainText,
            string password = null,
            DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Текст для шифрования не может быть пустым");

            try
            {
                // Используем фиксированную UTF-8 кодировку
                var data = _encoding.GetBytes(plainText);

                // Дополнительная соль (пароль)
                var salt = !string.IsNullOrEmpty(password)
                    ? _encoding.GetBytes(password)
                    : Array.Empty<byte>();

                // Шифрование через DPAPI
                var cipher = ProtectedData.Protect(data, salt, scope);

                // Возвращаем в Base64
                return Convert.ToBase64String(cipher);
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"Ошибка шифрования: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Расшифровывает текст с использованием DPAPI
        /// </summary>
        public string Decrypt(
            string cipherText,
            string password = null,
            DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Зашифрованный текст не может быть пустым");

            try
            {
                // Преобразуем из Base64
                var cipher = Convert.FromBase64String(cipherText);

                // Соль (должна совпадать с той, что использовалась при шифровании)
                var salt = !string.IsNullOrEmpty(password)
                    ? _encoding.GetBytes(password)
                    : Array.Empty<byte>();

                // Расшифровка через DPAPI
                var data = ProtectedData.Unprotect(cipher, salt, scope);

                // Возвращаем расшифрованный текст
                return _encoding.GetString(data);
            }
            catch (CryptographicException)
            {
                // Ошибка обычно означает неверный пароль или поврежденные данные
                throw new CryptographicException(
                    "Ошибка расшифровки. Проверьте пароль и целостность данных.");
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"Ошибка расшифровки: {ex.Message}", ex);
            }
        }
    }
}
