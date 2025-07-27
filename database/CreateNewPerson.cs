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
        CreateNew createNew;
        public CreateNewPerson(CastorCommonContext DatabaseContext)
        {
            db= DatabaseContext;
            createNew.DialogOK += Save;
            
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
