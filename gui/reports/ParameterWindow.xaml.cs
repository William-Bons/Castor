using Castor.gui.reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.reports
{
    public partial class ParameterWindow : Window
    {
        private readonly Dictionary<string, ReportParameter> _parameters;
        private readonly Dictionary<string, UIElement> _parameterControls = new Dictionary<string, UIElement>();
        public Dictionary<string, object> UpdatedValues { get; private set; } = new Dictionary<string, object>();

        public ParameterWindow(Dictionary<string, ReportParameter> parameters)
        {
            InitializeComponent();
            _parameters = parameters;
            GenerateParameterControls();
        }

        private void GenerateParameterControls()
        {
            ParametersPanel.Children.Clear();

            foreach (var kvp in _parameters)
            {
                var param = kvp.Value;
                var control = CreateControlForParameter(param);
                _parameterControls[param.Name] = control;

                // Добавляем в панель
                var stackPanel = new StackPanel();

                // Заголовок с именем параметра
                var label = new TextBlock
                {
                    Text = $"{param.Name}",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                };

                // Подсказка с типом
                var typeHint = new TextBlock
                {
                    Text = $"Тип: {param.Type.Name}, Текущее: {param.Value ?? "null"}",
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                stackPanel.Children.Add(label);
                stackPanel.Children.Add(typeHint);

                // Обертка для контроля с отступом
                var border = new Border
                {
                    Child = control,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                stackPanel.Children.Add(border);

                ParametersPanel.Children.Add(stackPanel);
            }
        }

        private UIElement CreateControlForParameter(ReportParameter param)
        {
            // Определяем базовый тип
            var type = param.Type;
            var value = param.Value;

            if (param.Items != null && param.Items.Count > 0)
            {
                return CreateComboBox(param);
            }
            // иначе построение контролла под тип параметра
            else return type.Name switch
            {
                "DateTime" => CreateDatePicker(param),
                "Boolean" => CreateCheckBox(param),
                "Int32" or "Int64" or "Int16" or "Decimal" or "Double" or "Single"
                    => CreateNumericTextBox(param),
                "Guid" => CreateTextBox(param, typeof(Guid)),
                _ => CreateTextBox(param, typeof(string))
            };
        }

        private UIElement CreateComboBox(ReportParameter param)
        {
            ComboBox combo = new ComboBox()
            {
                ItemsSource = param.Items,
                SelectedValuePath = "ID",
                DisplayMemberPath = "Value",
                SelectedValue = param.Value
            };
            return combo;
        }

        private UIElement CreateDatePicker(ReportParameter param)
        {
            var picker = new DatePicker
            {
                Width = 250,
                HorizontalAlignment = HorizontalAlignment.Left,
                SelectedDate = param.Value as DateTime? ?? DateTime.Now
            };
            return picker;
        }

        private UIElement CreateCheckBox(ReportParameter param)
        {
            var checkBox = new CheckBox
            {
                Content = "Включено",
                FontSize = 14,
                IsChecked = param.Value as bool? ?? false,
                Margin = new Thickness(0, 5, 0, 10)
            };
            return checkBox;
        }

        private UIElement CreateNumericTextBox(ReportParameter param)
        {
            var textBox = new TextBox
            {
                Width = 250,
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = param.Value?.ToString() ?? "0"
            };

            textBox.PreviewTextInput += (s, e) =>
            {
                e.Handled = !int.TryParse(e.Text, out _);
            };

            textBox.TextChanged += (s, e) =>
            {
                var tb = s as TextBox;
                if (!string.IsNullOrEmpty(tb.Text))
                {
                    if (!int.TryParse(tb.Text, out _))
                    {
                        tb.Text = "0";
                        tb.SelectAll();
                    }
                }
            };

            return textBox;
        }

        private UIElement CreateTextBox(ReportParameter param, Type targetType)
        {
            var textBox = new TextBox
            {
                Width = 350,
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = param.Value?.ToString() ?? string.Empty
            };
            return textBox;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Собираем значения
                UpdatedValues.Clear();

                foreach (var kvp in _parameterControls)
                {
                    var paramName = kvp.Key;
                    var control = kvp.Value;
                    var param = _parameters[paramName];

                    var value = GetValueFromControl(control, param.Type);
                    UpdatedValues[paramName] = value;
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private object GetValueFromControl(UIElement control, Type targetType)
        {
            return control switch
            {
                ComboBox box => box.SelectedValue ?? 0,
                DatePicker picker => picker.SelectedDate ?? DateTime.Now,
                CheckBox checkBox => checkBox.IsChecked ?? false,
                TextBox textBox => ConvertValue(textBox.Text, targetType),
                _ => null
            };
        }

        private object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
                return targetType == typeof(string) ? string.Empty : null;

            try
            {
                if (targetType == typeof(string)) return value;
                if (targetType == typeof(DateTime)) return DateTime.Parse(value);
                if (targetType == typeof(int)) return int.Parse(value);
                if (targetType == typeof(bool)) return bool.Parse(value);
                if (targetType == typeof(decimal)) return decimal.Parse(value);
                if (targetType == typeof(double)) return double.Parse(value);
                if (targetType == typeof(float)) return float.Parse(value);
                if (targetType == typeof(Guid)) return Guid.Parse(value);
                return value;
            }
            catch
            {
                return value;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}