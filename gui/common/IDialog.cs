using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castor.gui.common
{
    public interface IDialog
    {
        /// <summary>
        /// Интерфейс для класса Window показывающий что класс необходимо выводить как диалог
        /// </summary>
        public void Show();
    }
}
