using System.Collections.Generic;
using System.Windows.Controls;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public class ClassDropDown : InputElement<ComboBox, string>
    {
        /// <summary>
        ///     Returns a new Instance of TeacherDropdown.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public ClassDropDown(Tag parent, string name, string description, bool obligatory) : base(parent, name,
            description, obligatory, null, new ComboBox(), DataType.String)
        {
        }

        public override void SetStartValue()
        {
            ElevatedControl.ItemsSource = new List<string>
            {
                "FS161",
                "FV161",
                "FI161",
                "FI162"
            };
        }

        public override void SetValueFromString(string value)
        {
            ElevatedControl.SelectedValue = value;
        }

        public override bool Calculate(string value1, string value2, char operand)
        {
            return false;
        }

        public override bool ObligatoryCheck()
        {
            return ElevatedControl.SelectedIndex > -1;
        }

        public override string GetValue()
        {
            return (string) ElevatedControl.SelectedItem;
        }

        public override void Clear()
        {
            ElevatedControl.SelectedIndex = -1;
        }
    }
}