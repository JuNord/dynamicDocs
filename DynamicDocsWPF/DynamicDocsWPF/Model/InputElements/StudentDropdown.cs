using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.InputElements
{
    public class StudentDropdown : InputElement<ComboBox, string>
    {
        /// <summary>
        /// Returns a new Instance of StudentDropDown.
        /// </summary>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="isValidForProcess">Allows to supply a function to check if an Input is true.</param>
        public StudentDropdown(Tag parent, bool obligatory = false) : base(parent, obligatory, new ComboBox())
        {
            if (obligatory)
                ObligatoryCheck = () => ElevatedControl.SelectedIndex > -1;
            
            
            
            ValueToString = GetValue;
        }

        public override string GetValue() => (string) ElevatedControl.SelectedItem;

        public override void Clear() => ElevatedControl.SelectedIndex = -1;
        public override void Fill()
        {
            ElevatedControl.ItemsSource = new List<string>()
            {
                "Dennis Wüppelmann",
                "Julius Nordhues"
            };
        }

        public override bool CheckValidForControl() => true;
    }
}