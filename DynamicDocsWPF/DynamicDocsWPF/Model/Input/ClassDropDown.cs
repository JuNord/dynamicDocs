using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base;

namespace DynamicDocsWPF.Model.InputElements
{
    public class ClassDropDown : InputElement<ComboBox, string>
    {
        /// <summary>
        /// Returns a new Instance of TeacherDropdown.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public ClassDropDown(Tag parent, string name, string description,  bool obligatory = false) : base(parent, name, description, obligatory, new ComboBox())
        {
            ObligatoryCheck = () => ElevatedControl.SelectedIndex > -1;
        }
      
        public override void Fill()
        {
            ElevatedControl.ItemsSource = new List<string>()
            {
                "FS161",
                "FV161",
                "FI161",
                "FI162",
            };
        }
        
        public override string GetValue() => (string) ElevatedControl.SelectedItem;
        public override void Clear() => ElevatedControl.SelectedIndex = -1;
    }
}