using System;
using System.Windows.Controls;

namespace DynamicDocsWPF.Model.Base_Classes
{
    public abstract class InputElement<TControl, TValue> : BaseInputElement where TControl : Control 
    {
        protected TControl UiElement { get; }

        protected Func<bool> IsValidForProcess { private get; set; }
        protected Func<bool> FulfillsObligatory { private get; set; }

        protected InputElement(TControl control)
        {
            UiElement = control;
        }

        /// <summary>
        /// Returns whether the value of the underlying UI Control, fulfills certain InputElement specific conditions.
        /// e.g. If the TextBox of a NumberInputBox only includes digits
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckValidForControl();
        
        /// <summary>
        /// Returns the value of the underlying UI Control, according to its type
        /// </summary> 
        public abstract TValue GetValue();
        
        /// <summary>
        /// Clears the underlying UI Control, according to its type
        /// </summary>
        public abstract void Clear();
        
        /// <summary>
        /// Returns whether a controls content is fulfilling additional conditions defined in the XML
        /// </summary>
        /// <returns></returns>
        public bool CheckValidForProcess() => IsValidForProcess?.Invoke() ?? true;
        
        /// <summary>
        /// Returns whether a control fulfills any conditions about its obligatority
        /// </summary>
        /// <returns></returns>
        public bool CheckObligatory() => FulfillsObligatory?.Invoke() ?? true;
    }
}