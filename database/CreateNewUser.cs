using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;

namespace Castor.database
{

    public class CreateNewUser : IConsoleMessage, IDialog
    {
        public event ConsoleMessageHandler ConsoleMessage;

        CastorCommonContext db;
        CreateNew createNew;
        public CreateNewUser(CastorCommonContext DatabaseContext)
        {
            db = DatabaseContext;
            
        }

        public void Save()
        {
        }

        public void Show()
        {
            createNew.ShowDialog();
        }
    }
}
