

namespace Castor.gui.common
{
    public delegate void PrintStatusMessageHandler(string message, string barName);
    public interface IMainStatusBar
    {
        /// <summary>
        /// Интерфейс и делегат для класса нуждающегося в выводе сообщений на main StatusBar
        /// </summary>
        public event PrintStatusMessageHandler PrintStatusMessage;
    }
}
