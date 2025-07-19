using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;

namespace Castor.database
{

    public class CreateNewUser : IConsoleMessage, IDialog
    {
        public event ConsoleMessageHandler ConsoleMessage;

        CastorCommonContext db;
        private User User = new User();
        CreateNew createNew;
        public CreateNewUser(CastorCommonContext DatabaseContext)
        {
            db = DatabaseContext;
            createNew = new CreateNew(User);
            createNew.DialogOK += Save;
            
        }

        public void Save()
        {
            db.Users.Add(User);
            db.SaveChanges();
            ConsoleMessage?.Invoke($"user {User.Name} created");
        }

        public void Show()
        {
            createNew.ShowDialog();
        }
    }
}
