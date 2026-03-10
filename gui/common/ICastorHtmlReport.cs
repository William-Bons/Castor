using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.gui.common
{
    public interface ICastorHtmlReport
    {
        public string HtmlReport { get; }
        public void Calculate();
    }
}
