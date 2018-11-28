using System.Collections.Generic;
using System.Windows.Controls;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public class StudentDropdown : InputElement<ComboBox, string>
    {
        /// <summary>
        /// Returns a new Instance of StudentDropDown.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public StudentDropdown(Tag parent, string name, string description, bool obligatory = false) : base(parent, name, description, obligatory, new ComboBox())
        {
            ObligatoryCheck = () => ElevatedControl.SelectedIndex > -1;
        }

        public override string GetValue() => (string) ElevatedControl.SelectedItem;

        public override void Clear() => ElevatedControl.SelectedIndex = -1;
        public override void Fill()
        {
            ElevatedControl.ItemsSource = new List<string>()
            {
                "Dennis Wüppelmann",
                "Julius Nordhues"
            };
        }
    }
}