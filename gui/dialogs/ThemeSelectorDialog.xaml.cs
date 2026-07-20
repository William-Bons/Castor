using System.Windows;
using Castor.gui.common;

namespace Castor.gui.dialogs
{
    public partial class ThemeSelectorDialog : Window, IDialog
    {
        public ThemeSelectorDialog()
        {
            InitializeComponent();
            UpdateCurrentThemeLabel();
        }

        private void CastorSoftButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.SwitchTheme(ThemeManager.ThemeCastorSoft);
            UpdateCurrentThemeLabel();
        }

        private void CastorGreenButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.SwitchTheme(ThemeManager.ThemeCastorGreen);
            UpdateCurrentThemeLabel();
        }

        private void UpdateCurrentThemeLabel()
        {
            string currentTheme = ThemeManager.GetCurrentTheme();
            string displayName = currentTheme == ThemeManager.ThemeCastorSoft
                ? "CastorSoft (Синяя)"
                : "CastorGreen (Зелёная)";
            CurrentThemeLabel.Text = $"Текущая тема: {displayName}";
        }
    }
}
