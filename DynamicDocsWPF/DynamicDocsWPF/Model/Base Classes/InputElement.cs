using System;
using System.Windows.Controls;

namespace DynamicDocsWPF.Model.Base_Classes
{
    public abstract class InputElement<TControl, TValue> : BaseInputElement where TControl : Control
    {
        public TControl ElevatedControl => BaseControl as TControl;


        protected InputElement(Tag parent, bool obligatory, TControl control) : base(parent, obligatory, control)
        {
        }

        /// <summary>
        /// Returns the value of the underlying UI Control, according to its type
        /// </summary> 
        public abstract TValue GetValue();
    }
}