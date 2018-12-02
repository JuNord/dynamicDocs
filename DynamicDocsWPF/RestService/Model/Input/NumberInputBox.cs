using System;
using System.CodeDom;
using System.Windows.Controls;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public class NumberInputBox : InputElement<TextBox, int>
    {
        /// <summary>
        ///     Returns a new Instance of NumberInputBox.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public NumberInputBox(Tag parent, string name, string description, bool obligatory = false) : base(parent, name,
            description, obligatory, new TextBox(), DataType.Int)
        {
            ObligatoryCheck = () => !string.IsNullOrEmpty(ElevatedControl.Text);

            ControlErrorMsg = "Bitte nur Zahlen eingeben.";
            ControlValidityCheck = () =>
            {
                var valid = true;
                foreach (var c in ElevatedControl.Text)
                    if (!char.IsDigit(c))
                        valid = false;

                return valid;
            };
        }

        public override int GetValue()
        {
            try
            {
                return int.Parse(ElevatedControl.Text);
            }
            catch (FormatException)
            {
                return int.MinValue;
            }
        }

        public override void Clear()
        {
            ElevatedControl.Clear();
        }

        public override void SetStartValue()
        {
            ElevatedControl.Text = "";
        }

        public override void SetValueFromString(string value)
        {
            ElevatedControl.Text = value;
        }
    }
}