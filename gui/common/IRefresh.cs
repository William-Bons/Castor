using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.gui.common
{
    public delegate void RefreshEventHandler(params string[] classes);
    public interface IRefresh
    {
        public event RefreshEventHandler RefreshNotify;
        public void Refresh();

    }
}
