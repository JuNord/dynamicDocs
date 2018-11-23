using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.InputElements
{
    public class ClassDropDown : InputElement<ComboBox, string>
    {
        /// <summary>
        /// Returns a new Instance of TeacherDropdown.
        /// </summary>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="isValidForProcess">Allows to supply a function to check if an Input is true.</param>
        public ClassDropDown(Tag parent, bool obligatory = false) : base(parent, obligatory, new ComboBox())
        {
            if (obligatory)
                FulfillsObligatory = () => ElevatedControl.SelectedIndex > -1;

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

        public override bool CheckValidForControl() => true;
    }
}