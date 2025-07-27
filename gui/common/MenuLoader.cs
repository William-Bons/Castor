using Castor.gui.common;
using System.Windows.Controls;
using System.Xml;

namespace Castor.gui
{
    public class MenuLoader
    {
        public delegate void MenuItemRiseEvent(CastorMenuItem sender);
        public event MenuItemRiseEvent MenuItemRise;

        private object _mainMenu;
        private string _menuFilePath;
        private MenuItem? _currentMenu = null;
        private MenuItem? _parentMenu = null;
        public MenuLoader(object MainMenu, string MenuFilePath = "assets/MainMenu.xml")
        {
            _mainMenu = MainMenu;
            _menuFilePath = MenuFilePath;
            LoadMainMenuFromTemplate();
        }
        private void LoadMainMenuFromTemplate()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(_menuFilePath);
            foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            {
                LoadItem(node,_mainMenu);
            }
        }
        
        private void LoadItem(XmlNode node,object parent)
        {
            if (node is null) return;
            
            else if (node.Name == "MenuItem" && node.HasChildNodes)
            {
                _parentMenu = _currentMenu is MenuItem mi ? mi : null;

                _currentMenu = new MenuItem();
                _currentMenu.Name = node?.Attributes?.GetNamedItem("Name")?.Value;
                _currentMenu.Header = node?.Attributes?.GetNamedItem("Header")?.Value;

                if (parent is ItemsControl _a) 
                    _a.Items.Add(_currentMenu);
                
                foreach (XmlNode _snode in node.ChildNodes)
                    LoadItem(_snode, _currentMenu);
                _currentMenu = _parentMenu;
            }
            
            else if (node.Name == "MenuItem" && node.Attributes?.GetNamedItem("d") == null)
            {
                CastorMenuItem menuItem = new CastorMenuItem();
                menuItem.Header = node.Attributes?.GetNamedItem("Header")?.Value;
                menuItem.ClassName = node.Attributes?.GetNamedItem("ClassName")?.Value;
                menuItem.Parameter = node.Attributes?.GetNamedItem("Parameter")?.Value;
                menuItem.InputGestureText = node.Attributes?.GetNamedItem("InputGestureText")?.Value;
                menuItem.Tag = node.Attributes?.GetNamedItem("Tag")?.Value;

                menuItem.Click += delegate
                {
                    MenuItemRise?.Invoke(menuItem); // here send event 
                };
                _currentMenu?.Items.Add(menuItem);
            }
            
            else if (node.Name == "Separator")
            {
                _currentMenu?.Items.Add(new Separator());
            }
        }
    }
}
