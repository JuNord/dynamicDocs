using System;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.InputElements
{
    public class NumberInputBox : InputElement<TextBox, string>
    {
        /// <inheritdoc />
        public NumberInputBox() : this(false)
        {
            
        }
        
        /// <summary>
        /// Returns a new Instance of NumberInputBox.
        /// </summary>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="isValidForProcess">Allows to supply a function to check if an Input is true.</param>
        public NumberInputBox(bool obligatory, Func<bool> isValidForProcess = null) : base(new TextBox())
        {
            IsValidForProcess = isValidForProcess;
            
            if(obligatory)
                FulfillsObligatory = () => UiElement.Text.Length > 0;
        }

        public override void Clear() => UiElement.Clear();
        
        public override bool CheckValidForControl()
        {
            var valid = true;
            foreach(var c in UiElement.Text)
                if (!char.IsDigit(c))
                    valid = false; 
            
            return valid;
        }

        public override string GetValue() => UiElement.Text;
    }
}