using System.Windows.Controls;

namespace Castor.gui.common
{
    public class CastorMenuItem : MenuItem
    {
        /// <summary>
        /// Расширяет стандартный класс MenuItem присоединяя свойства 
        /// </summary>
        public string? ClassName { get; set; }
        public object? Parameter { get; set; }
    }
}
