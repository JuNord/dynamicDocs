using System;
using System.Windows.Controls;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public class DateDropdown : InputElement<DatePicker, DateTime?>
    {
        /// <summary>
        ///     Returns a new Instance of TeacherDropdown.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="hasCalculation"></param>
        public DateDropdown(Tag parent, string name, string description, bool obligatory, string calculation) : base(
            parent, name,
            description, obligatory, calculation, new DatePicker(), DataType.DateTime)
        {
            ElevatedControl.SelectedDateChanged += delegate { ((Dialog) parent).PerformCalculations(); };
        }

        public override DateTime? GetValue()
        {
            return ElevatedControl.SelectedDate;
        }

        public override void Clear()
        {
            ElevatedControl.SelectedDate = DateTime.Now;
        }

        public override void SetStartValue()
        {
            ElevatedControl.SelectedDate = DateTime.Now;
        }

        public override void SetValueFromString(string value)
        {
            DateTime.TryParse(value, out var interpretedValue);
            ElevatedControl.SelectedDate = interpretedValue;
        }

        public override bool ObligatoryCheck()
        {
            return !string.IsNullOrWhiteSpace(ElevatedControl.Text);
        }
        
        public override bool Calculate(string value1, string value2, char operand)
        {
            var isValue1Number = double.TryParse(value1, out var value1Double);
            var isValue2Number = double.TryParse(value2, out var value2Double);
            var isValue1Date = DateTime.TryParse(value1, out var value1Date);
            var isValue2Date = DateTime.TryParse(value2, out var value2Date);

            int factor;
            switch (operand)
            {
                case '+':
                    factor = 1;
                    break;
                case '-':
                    factor = -1;
                    break;
                default: return false;
            }

            if (isValue1Date && isValue2Number)
            {
                value1Date = value1Date.AddDays(value2Double * factor);
                ElevatedControl.SelectedDate = value1Date;
                return true;
            }

            return false;
        }

        public override string GetFormattedValue()
        {
            return GetValue()?.ToShortDateString();
        }
    }
}