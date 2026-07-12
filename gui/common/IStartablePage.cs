using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Castor.gui.common
{
    public interface IStartablePage
    {
        public bool CanStart {  get; }
    }
}
