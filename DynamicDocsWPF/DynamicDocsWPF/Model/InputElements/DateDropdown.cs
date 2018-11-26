using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.InputElements
{
    public class DateDropdown : InputElement<DatePicker, string>
    {
        /// <summary>
        /// Returns a new Instance of TeacherDropdown.
        /// </summary>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="isValidForProcess">Allows to supply a function to check if an Input is true.</param>
        public DateDropdown(Tag parent, bool obligatory = false) : base(parent, obligatory, new DatePicker())
        {
            if (obligatory)
                ObligatoryCheck = () => !string.IsNullOrWhiteSpace(ElevatedControl.Text);
            
            ValueToString = GetValue;
        }

        public override string GetValue() => ElevatedControl.SelectedDate?.ToShortDateString();

        public override void Clear() => ElevatedControl.SelectedDate = DateTime.Now;
        
        public override void Fill()
        {
            ElevatedControl.SelectedDate = DateTime.Now;
        }

        public override bool CheckValidForControl() => true;
    }
}