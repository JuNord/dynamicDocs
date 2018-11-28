using System.Windows.Controls;
using RestService.Model.Base;

namespace RestService.Model.Input
{
    public abstract class InputElement<TControl, TValue> : BaseInputElement where TControl : Control
    {
        protected InputElement(Tag parent, string name, string description, bool obligatory, TControl control) : base(
            parent, name, description, obligatory, control)
        {
            GetFormattedValue = () => GetValue() as string ?? throw new MissingValueInterpretationException();
        }

        public TControl ElevatedControl => BaseControl as TControl;

        /// <summary>
        ///     Returns the value of the underlying UI Control, according to its type
        /// </summary>
        public abstract TValue GetValue();
    }
}