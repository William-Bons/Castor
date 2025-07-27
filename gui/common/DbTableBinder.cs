using Castor.database;
using Castor.database.tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Castor.gui
{
    public class DbTableBinder
    {
        public DbTableBinder(CastorCommonContext Db, MetaTable TableObject, Grid grid)
        {
            // define Grid columns 1/2 and 2/3
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            
            // get all properties (Table columns) 
            var allProperties = TableObject.getInfo();
            int rown = 0;
            
            foreach (var propertie in allProperties)
            {
                // add new grid row
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

                // gets properties attributes
                string[] attributes = { };
                if (propertie.CustomAttributes.Count() > 0 &&
                    propertie.CustomAttributes.First().ConstructorArguments.Count > 0)
                {   
                    attributes = propertie.CustomAttributes.First().ConstructorArguments[0].Value?.ToString().Split(';');
                }

                // if attributes contains 'no' - skip this field
                if (attributes?.Length>0 &&  (attributes.Contains<string>("no") ||
                    attributes[0].Contains("ReadOnly"))) continue;

                // make LABEL
                TextBlock textBlock = new TextBlock() { Text = propertie.Name };
                textBlock.Padding = new Thickness(3);
                Grid.SetRow(textBlock, rown);
                grid.Children.Add(textBlock);

                var a = propertie.PropertyType;

                // make UI element regarding attribute description
                UIElement uI;
                if(attributes?.Length>0 && attributes.Contains("Date"))
                {
                    uI = new DatePicker();
                    ((DatePicker)uI).Margin = new Thickness(3);
                    Binding binding1 = new Binding(propertie.Name);
                    binding1.Source = TableObject;
                    ((DatePicker)uI).SetBinding(DatePicker.SelectedDateProperty, binding1);
                }
                else if(attributes?.Length>0 && attributes.Contains("Combo"))
                {
                    uI = new ComboBox();
                    ((ComboBox)uI).Margin = new Thickness(3);
                }
                else
                {
                    uI = new TextBox();
                    ((TextBox)uI).Margin = new Thickness(3);
                    ((TextBox)uI).Padding = new Thickness(2);
                    Binding binding = new Binding(propertie.Name);
                    binding.Source = TableObject;
                    ((TextBox)uI).SetBinding(TextBox.TextProperty, binding);
                }

                Grid.SetRow(uI, rown++);
                Grid.SetColumn(uI, 1);
                grid.Children.Add(uI);
            }
        }

        public object Get(CastorCommonContext Db, string TableName)
        {
            return null;
        }
    }
}
