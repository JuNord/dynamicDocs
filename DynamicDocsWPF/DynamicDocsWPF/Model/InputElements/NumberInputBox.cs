using System;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.InputElements
{
    public class NumberInputBox : InputElement<TextBox, int>
    {
        /// <summary>
        /// Returns a new Instance of NumberInputBox.
        /// </summary>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="isValidForProcess">Allows to supply a function to check if an Input is true.</param>
        public NumberInputBox(Tag parent, bool obligatory = false) : base(parent, obligatory, new TextBox())
        {
            if(obligatory)
                ObligatoryCheck = () => !string.IsNullOrEmpty(ElevatedControl.Text);

            ControlErrorMsg = "Bitte nur Zahlen eingeben.";
            
            ValueToString = GetValue().ToString;
        }

        public override void Clear() => ElevatedControl.Clear();
        
        public override void Fill()
        {
            ElevatedControl.Text = "";
        }

        public override bool CheckValidForControl()
        {
            var valid = true;
            foreach(var c in ElevatedControl.Text)
                if (!char.IsDigit(c))
                    valid = false; 
            
            return valid;
        }

        public override int GetValue() => int.Parse(ElevatedControl.Text);
    }
}