using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Castor.gui.common
{
    class MoveFocusHelper
    {
        public MoveFocusHelper(Panel PanelElementsOfFocus, Key[] Keys, Action<object> ActionOnEnter, Action ActionOnExit)
        {

            foreach (FrameworkElement CurrentElement in PanelElementsOfFocus.Children)
            {
                // Если Combo привязка события SelectionChanged к перемещению фокуса
                if (CurrentElement is ComboBox _cbel)
                {
                    _cbel.SelectionChanged += delegate
                    {
                        _cbel.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    };
                }

                // Если элемент DatePicker
                if (CurrentElement is DatePicker _dp)
                {
                    _dp.SelectedDateChanged += delegate
                    {
                        ((FrameworkElement)_dp.PredictFocus(FocusNavigationDirection.Down))?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    };
                    _dp.CalendarClosed += delegate
                    {
                        ((FrameworkElement)_dp.PredictFocus(FocusNavigationDirection.Down))?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    };
                }

                // Реакции на нажатие определенных клавиш
                if (CurrentElement is UIElement _kde)
                {
                    _kde.KeyDown += (object src, KeyEventArgs args) =>
                    {
                        // Если нажата Escape
                        if (args.Key == Key.Escape && src is UIElement _ui)
                        {
                            _ui.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                            ActionOnExit?.Invoke();
                        }

                        // Если нажата клавиша из массива Keys[]
                        if (Keys.Contains(args.Key))
                        {
                            // для TextBox сохранение введенных данных 
                            if (src is TextBox _tbsrc)
                            {
                                _tbsrc.Text = _tbsrc.Text.Trim();           // remove all spaces from end and start
                                _tbsrc.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                            }

                            // для любого вводного элемента панели - перемещение фокуса на следующий вводный элемент
                            if (src is UIElement _el)
                            {
                                _el.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                            }

                            ActionOnEnter?.Invoke(src);
                        }
                    };
                }
            }

            PanelElementsOfFocus.Loaded += delegate //moves keyboard focus on first element in Panel
            {
                PanelElementsOfFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            };

        }

    }
}
