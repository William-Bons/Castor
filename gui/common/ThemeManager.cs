using System.Windows;
using Castor.Properties;

namespace Castor.gui.common
{
    /// <summary>
    /// Управляет переключением между темами (CastorSoft и CastorGreen)
    /// </summary>
    public static class ThemeManager
    {
        // Доступные темы
        public const string ThemeCastorSoft = "CastorSoft";
        public const string ThemeCastorGreen = "CastorGreen";

        /// <summary>
        /// Переключает тему приложения
        /// </summary>
        /// <param name="themeName">Имя темы: CastorSoft или CastorGreen</param>
        public static void SwitchTheme(string themeName)
        {
            string themeUri = themeName switch
            {
                ThemeCastorSoft => "/assets/CastorSoft.xaml",
                ThemeCastorGreen => "/assets/CastorGreen.xaml",
                _ => "/assets/CastorSoft.xaml"
            };

            try
            {
                // Удаляем старую тему из MergedDictionaries
                var currentDict = Application.Current.Resources.MergedDictionaries
                    .FirstOrDefault(d => d.Source?.OriginalString.EndsWith(".xaml") == true 
                                      && (d.Source.OriginalString.Contains("CastorSoft") 
                                          || d.Source.OriginalString.Contains("CastorGreen")));

                if (currentDict != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(currentDict);
                }

                // Добавляем новую тему
                var newTheme = new ResourceDictionary { Source = new Uri(themeUri, UriKind.Relative) };
                Application.Current.Resources.MergedDictionaries.Add(newTheme);

                // Сохраняем выбор в настройки
                Settings.Default.CurrentTheme = themeName;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка переключения темы: {ex.Message}");
            }
        }

        /// <summary>
        /// Загружает сохранённую тему при старте приложения
        /// </summary>
        public static void LoadSavedTheme()
        {
            string savedTheme = Settings.Default.CurrentTheme;
            if (!string.IsNullOrWhiteSpace(savedTheme))
            {
                SwitchTheme(savedTheme);
            }
        }

        /// <summary>
        /// Получает текущую активную тему
        /// </summary>
        public static string GetCurrentTheme()
        {
            var currentDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source?.OriginalString.Contains("CastorSoft") == true);

            if (currentDict != null)
                return ThemeCastorSoft;

            currentDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source?.OriginalString.Contains("CastorGreen") == true);

            if (currentDict != null)
                return ThemeCastorGreen;

            return ThemeCastorSoft;
        }
    }
}
