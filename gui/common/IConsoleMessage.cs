namespace Castor.gui.common
{
    public delegate void ConsoleMessageHandler(string message);
    public interface IConsoleMessage
    {
        public event ConsoleMessageHandler ConsoleMessage;
    }
}
