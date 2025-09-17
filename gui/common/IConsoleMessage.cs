

namespace Castor.gui.common
{
    public delegate void ConsoleMessageHandler(string message);
    public interface IConsoleMessage
    {
        /// <summary>
        /// Интерфейс класса для вывода сообщений в консоль CastorConsole
        /// </summary>
        public event ConsoleMessageHandler ConsoleMessage;
    }
}
