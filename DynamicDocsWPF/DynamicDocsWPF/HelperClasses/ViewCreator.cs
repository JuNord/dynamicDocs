using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using RestService.Model.Input;

namespace DynamicDocsWPF.HelperClasses
{
    public static class ViewCreator
    {
        private static Grid GetView(BaseInputElement element)
        {
            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(20, 0, 20, 20),
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    },
                    new ColumnDefinition()
                }
            };

            var label = new Label
            {
                Content = element.Description,
                FontSize = 13,
                Margin = new Thickness(0, 0, 20, 0)
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
            for (var i = 0; i < dialog.ElementCount; i++) viewHolder.Children.Add(GetView(dialog.GetElementAtIndex(i)));
        }
        
        public static Grid GetViewTable(List<BaseInputElement> elements)
        {
            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 0, 0, 0) 
            };
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            
            var stackpanel = new StackPanel();
            grid.Children.Add(stackpanel);
            
            int i = 0;
            foreach (var baseInputElement in elements)
            {  
                var label = new Label
                {
                    Content = baseInputElement.Description,
                    FontSize = 13,
                    Margin = new Thickness(0, 0, 20, 0)
                   
                };
                if (baseInputElement.Obligatory)
                    label.Content += " *";
                
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                  
                grid.Children.Add(label);

                label.SetValue(Grid.RowProperty, 0);
                label.SetValue(Grid.ColumnProperty, i);
                stackpanel.SetValue(Grid.RowProperty, 1);
                
                i++;
            }
            
            stackpanel.SetValue(Grid.ColumnSpanProperty, i+1);

            stackpanel.Children.Add(GetTableRow(elements));
            
            return grid;
        }

        
        public static Grid GetTableRow(List<BaseInputElement> elements)
        {
            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(20, 0, 20, 20),
            };

            int i = 0;
            foreach (var baseInputElement in elements)
            {
                ColumnDefinition col=new ColumnDefinition();
                grid.ColumnDefinitions.Add(col);
                baseInputElement.BaseControl.SetValue(Grid.ColumnProperty, i);
                i++;
                grid.Children.Add(baseInputElement.BaseControl);
            }
            return grid;
        }
    }
}