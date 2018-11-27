using System.Windows.Controls;
using DynamicDocsWPF.Model.Base;

namespace DynamicDocsWPF.Model.InputElements
{
    public abstract class InputElement<TControl, TValue> : BaseInputElement where TControl : Control
    {
        public TControl ElevatedControl => BaseControl as TControl;

        protected InputElement(Tag parent, string name, string description,  bool obligatory, TControl control) : base(parent, name, description, obligatory, control)
        {
        }

        /// <summary>
        /// Returns the value of the underlying UI Control, according to its type
        /// </summary> 
        public abstract TValue GetValue();
    }
}