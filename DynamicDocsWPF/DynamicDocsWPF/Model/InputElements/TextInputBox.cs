using System;
using System.Windows.Controls;
using DynamicDocsWPF.Model.Base_Classes;

namespace DynamicDocsWPF.Model.InputElements
{
    public class TextInputBox : InputElement<TextBox, string>
    {
        /// <summary>
        /// Returns a new Instance of TextInputBox.
        /// </summary>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        public TextInputBox(Tag parent, bool obligatory = false) : base(parent, obligatory, new TextBox())
        {
            if(obligatory)
                FulfillsObligatory = () => ElevatedControl.Text.Length > 0;
        }

        public override void Clear() => ElevatedControl.Clear();

        public override bool CheckValidForControl() => true;

        public override string GetValue() => ElevatedControl.Text;
    }
}