using System;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.InputElements
{
    public class TextInputBox : InputElement<TextBox, string>
    {
        /// <inheritdoc />
        public TextInputBox() : this(false)
        {
            
        }
        
        /// <summary>
        /// Returns a new Instance of TextInputBox.
        /// </summary>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="isValidForProcess">Allows to supply a function to check if an Input is true.</param>
        public TextInputBox(bool obligatory, Func<bool> isValidForProcess = null) : base(new TextBox())
        {
            IsValidForProcess = isValidForProcess;
            
            if(obligatory)
                FulfillsObligatory = () => UiElement.Text.Length > 0;
        }

        public override void Clear() => UiElement.Clear();

        public override bool CheckValidForControl() => true;

        public override string GetValue() => UiElement.Text;
    }
}