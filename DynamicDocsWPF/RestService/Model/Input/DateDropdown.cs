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
        public DateDropdown(Tag parent, string name, string description, bool obligatory = false) : base(parent, name,
            description, obligatory, new DatePicker(), DataType.DateTime)
        {
            ObligatoryCheck = () => !string.IsNullOrWhiteSpace(ElevatedControl.Text);
            GetFormattedValue = () => GetValue()?.ToShortDateString();
        }

        public override DateTime? GetValue()
        {
            return ElevatedControl.SelectedDate;
        }

        public override void Clear()
        {
            ElevatedControl.SelectedDate = DateTime.Now;
        }

        public override void Fill()
        {
            ElevatedControl.SelectedDate = DateTime.Now;
        }
    }
}