using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.gui.common
{
    public delegate void SwitchPageHandler(string className, object param);
    public interface ISwithPage
    {
        /// <summary>
        /// Интерфейс для реализации переключения страниц на майнфрейме из дочерней страницы
        /// </summary>
        public event SwitchPageHandler SwitchPage;
    }
}
