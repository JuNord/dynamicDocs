using System.Windows.Controls;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public class TextInputBox : InputElement<TextBox, string>
    {
        /// <summary>
        ///     Returns a new Instance of TextInputBox.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="obligatory">If true, a check function will be supplied to the base class to check if the control is empty</param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        public TextInputBox(Tag parent, string name, string description, bool obligatory) : base(parent, name,
            description, obligatory, null, new TextBox(), DataType.String)
        {
        }

        public override void Clear()
        {
            ElevatedControl.Clear();
        }

        public override void SetStartValue()
        {
            ElevatedControl.Text = "";
        }

        public override string GetValue()
        {
            return ElevatedControl.Text;
        }

        public override void SetValueFromString(string value)
        {
            ElevatedControl.Text = value;
        }

        public override bool Calculate(string value1, string value2, char operand)
        {
            return false;
        }

        public override bool ObligatoryCheck()
        {
            return !string.IsNullOrWhiteSpace(ElevatedControl.Text);
        }
    }
}