using System;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.InputElements
{
    public class StudentDropdown : InputElement<ComboBox, string>
    {
        /// <inheritdoc />
        public StudentDropdown() : this(false)
        {
        }

        /// <summary>
        /// Returns a new Instance of StudentDropDown.
        /// </summary>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="isValidForProcess">Allows to supply a function to check if an Input is true.</param>
        public StudentDropdown(bool obligatory, Func<bool> isValidForProcess = null) : base(new ComboBox())
        {
            IsValidForProcess = isValidForProcess;

            if (obligatory)
                FulfillsObligatory = () => UiElement.SelectedIndex > -1;
        }

        public override string GetValue() => (string) UiElement.SelectedItem;

        public override void Clear() => UiElement.SelectedIndex = -1;

        public override bool CheckValidForControl() => true;
    }
}