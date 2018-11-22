using System.Windows;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base_Classes;
using DynamicDocsWPF.Model.Surrounding_Tags;

namespace DynamicDocsWPF.UIGeneration
{
    public class ViewCreator
    {
        public static Grid GetView(BaseInputElement element)
        {
            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(20, 0, 20, 20),
                ColumnDefinitions =
                {
                    new ColumnDefinition()
                    {
                        Width = GridLength.Auto
                    },
                    new ColumnDefinition()
                    
                }
            };

            var label = new Label()
                        {
                            Content = element.Description,
                            FontSize = 13,
                            Margin = new Thickness(0,0,20,0)
                        };
            if (element.Obligatory)
                label.Content += " *";

            grid.Children.Add(label);
            grid.Children.Add(element.BaseControl);
            
            label.SetValue(Grid.ColumnProperty, 0);
            element.BaseControl.SetValue(Grid.ColumnProperty, 1);
            
            return grid;
        }

        public static void FillViewHolder(StackPanel viewHolder, Dialog dialog)
        {
            viewHolder.Children.Clear();
            for (var i = 0; i < dialog.GetElementCount(); i++)
            {
                viewHolder.Children.Add(GetView(dialog.GetElementAtIndex(i)));
            }
            
        }
    }
}