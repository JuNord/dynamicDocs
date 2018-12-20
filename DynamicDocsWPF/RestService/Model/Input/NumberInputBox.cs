using System;
using System.Windows.Controls;
using DynamicDocsWPF.Model;
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
        public NumberInputBox(Tag parent, string name, string description, bool obligatory, string calculation) : base(
            parent, name,
            description, obligatory, calculation, new TextBox(), DataType.Int)
        {
            ControlErrorMsg = "Bitte nur Zahlen eingeben.";
            ControlValidityCheck = () =>
            {
                var valid = true;
                foreach (var c in ElevatedControl.Text)
                    if (!char.IsDigit(c))
                        valid = false;

                return valid;
            };
            ElevatedControl.TextChanged += delegate { ((Dialog) parent).PerformCalculations(); };
        }

        public override int GetValue()
        {
            try
            {
                return string.IsNullOrWhiteSpace(ElevatedControl.Text) ? 0 : int.Parse(ElevatedControl.Text);
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

        public override bool Calculate(string value1, string value2, char operand)
        {
            double value1Double = 0;
            double value2Double = 0;
            var isValue1Date = DateTime.TryParse(value1, out var value1Date);
            var isValue2Date = DateTime.TryParse(value2, out var value2Date);
            var isValue1Number = !isValue1Date && double.TryParse(value1, out value1Double);
            var isValue2Number = !isValue2Date && double.TryParse(value2, out value2Double);


            if (isValue1Number && isValue2Number)
            {
                double result;
                switch (operand)
                {
                    case '+':
                        result = value1Double + value2Double;
                        break;
                    case '-':
                        result = value1Double - value2Double;
                        break;
                    case '*':
                        result = value1Double * value2Double;
                        break;
                    case '/':
                        result = value1Double / value2Double;
                        break;
                    default: return false;
                }

                ElevatedControl.Text = $"{result}";
                return true;
            }

            if (isValue1Date && isValue2Date && value1Date > value2Date)
            {
                double result = value2Date.BusinessDaysUntil(value1Date);
                ElevatedControl.Text = $"{result}";
                return true;
            }

            return false;
        }

        public override bool ObligatoryCheck()
        {
            return !string.IsNullOrEmpty(ElevatedControl.Text);
        }
    }
}