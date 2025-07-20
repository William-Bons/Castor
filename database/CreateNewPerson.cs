using Castor.database.tables;
using Castor.gui.common;
using Castor.gui.dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.database
{
    public class CreateNewPerson : IConsoleMessage, IDialog
    {
        public event ConsoleMessageHandler ConsoleMessage;
        CastorCommonContext db;
        private User User = new User();
        private Person Person = new Person();
        CreateNew createNew;
        public CreateNewPerson(CastorCommonContext DatabaseContext)
        {
            db= DatabaseContext;
            createNew = new CreateNew(DatabaseContext, Person);
            createNew.DialogOK += Save;
            
        }
        public void Save()
        {
            db.Persons.Add(Person);
            db.SaveChanges();
            ConsoleMessage?.Invoke($"person {Person.FullName} created");
        }

        public void Show()
        {
            createNew.ShowDialog();
        }
    }
}
