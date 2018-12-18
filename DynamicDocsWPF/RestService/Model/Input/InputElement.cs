using System.Windows.Controls;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public abstract class InputElement<TControl, TValue> : BaseInputElement where TControl : Control
    {
        protected InputElement(Tag parent, string name, string description, bool obligatory, string calculation,
            TControl control,
            DataType dataType) : base(
            parent, name, description, obligatory, calculation, control)
        {
        }

        public TControl ElevatedControl => BaseControl as TControl;

        /// <summary>
        ///     Returns the value of the underlying UI Control, according to its type
        /// </summary>
        public abstract TValue GetValue();

        /// <inheritdoc />
        /// <summary>
        ///     Returns the value of the underlying UI Control, formatted as a string.
        ///     When not overridden, tries to safely cast the value to string and throws Exception on failure
        /// </summary>
        /// <returns></returns>
        public override string GetFormattedValue()
        {
            return GetValue().ToString();
        }
    }
}