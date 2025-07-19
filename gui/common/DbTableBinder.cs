using Castor.database.tables;
using System.CodeDom;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Castor.gui
{
    public class DbTableBinder
    {
        public DbTableBinder(MetaTable TableObject, Grid grid)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width=new GridLength(1, GridUnitType.Star)});
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width=new GridLength(2, GridUnitType.Star)});

            var a = TableObject.getInfo();
            int rown = 0;
            foreach (var item in a)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

                if (item.CustomAttributes.Count() > 0 &&
                    item.CustomAttributes.First().ConstructorArguments[0].Value?.ToString() == "no")
                    continue;

                TextBlock textBlock = new TextBlock() { Text = item.Name };
                textBlock.Padding = new System.Windows.Thickness(3);
                Grid.SetRow(textBlock, rown);
                grid.Children.Add(textBlock);

                UIElement uI;
                switch(item.PropertyType.Name)
                {
                    case "DateTime":
                        uI = new DatePicker();
                        ((DatePicker)uI).Margin = new System.Windows.Thickness(3);
                        Binding binding1 = new Binding(item.Name);
                        binding1.Source = TableObject;
                        ((DatePicker)uI).SetBinding(DatePicker.SelectedDateProperty, binding1);

                        break;

                    default:
                        uI = new TextBox();
                        ((TextBox)uI).Margin = new System.Windows.Thickness(3);
                        Binding binding = new Binding(item.Name);
                        binding.Source = TableObject;
                        ((TextBox)uI).SetBinding(TextBox.TextProperty, binding);
                        
                        break;
                }

                Grid.SetRow(uI, rown++);
                Grid.SetColumn(uI, 1);
                grid.Children.Add(uI);
            }
        }
    }
}
